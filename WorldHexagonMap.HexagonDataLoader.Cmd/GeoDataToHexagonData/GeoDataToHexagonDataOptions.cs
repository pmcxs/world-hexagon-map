using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.Geojson.GeoDataParser;
using WorldHexagonMap.HexagonDataLoader.Geotiff.GeoDataParser;
using WorldHexagonMap.HexagonDataLoader.HexagonDataExporters;
using WorldHexagonMap.HexagonDataLoader.Shapefiles.GeoDataParser;
using WorldHexagonMap.HexagonDataLoader.SQLite.HexagonDataExport;

namespace WorldHexagonMap.HexagonDataLoader.Cmd.GeoDataToHexagonData
{
    [Verb("geo2hexagon", HelpText = "Reads geographical data and generates hexagonal information")]
    internal class GeoDataToHexagonDataOptions
    {
        [Option('v', "verbose", HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "input")]
        public string Input { get; set; }
        
        [Option(longName:"inputtype")]
        public InputType InputType { get; set; }
        
        [Option('t', "targets")]
        public IEnumerable<string> Targets { get; set; }
        
        [Option('f', "filters")]
        public IEnumerable<string> Filters { get; set; }

        [Option('m', "merge", Required = false, Default = MergeStrategy.Replace, HelpText = "Defines how a new value should be merged with an existing one" )]
        public MergeStrategy Merge { get; set; }

        [Option('o',"output", Required = false)]
        public string Output { get; set; }

        [Option('h', "hexagonsize", Default = 10)]
        public double HexagonSize { get; set; }
        
        [Option("referencezoom", Default = 10)]
        public int HexagonReferenceZoom { get; set; }

        [Option("outputtype", Required = false, Default = OutputType.Console)]
        public OutputType OutputType { get; set; }

        [Option("interpolate", Default = true)]
        public bool Interpolate { get; set; }

        internal LayersConfiguration GetLayersConfiguration()
        {
            var targets = new List<LayersLoaderTarget>();

            foreach (string target in Targets)
            {
                string[] lines = target.Split(":");

                //country:iso_a2
                //river 
                //altitude:handler(wikimedia_altitude)

                LayersLoaderTarget loaderTarget = new LayersLoaderTarget
                {
                    Destination = lines[0]
                };


                if (lines.Length == 1)
                {
                    loaderTarget.SourceField = lines[0];
                }
                else
                {
                    Regex regex = new Regex("handler\\((?<handler>\\w+)\\)");
                    Match m = regex.Match(lines[1]);

                    if (m.Success)
                    {
                        loaderTarget.Handler = m.Groups["handler"].Value;
                    }
                    else
                    {
                        loaderTarget.SourceField = lines[1];
                    }
                }

                targets.Add(loaderTarget);
            }

            var filters = new List<LayersLoaderFilter>();

            foreach (string filter in Filters)
            {
                string[] lines = filter.Split(":");

                if (lines.Length != 2)
                {
                    throw new NotSupportedException($"A filter needs 2 parts: <field>:<value-to-include>");
                }
                
                filters.Add(new LayersLoaderFilter { Field = lines[0], Value = lines[1]});
                
            }
            
            var layers = new LayersConfiguration
            {
                Source = Input, 
                Targets = targets.ToArray(),
                Filters = filters.ToArray(),
                Interpolate = Interpolate
            };

            return layers;

        }

        internal IGeoDataParser GetGeoDataParser()
        {
            if (InputType == InputType.None)
            {
                var extension = Path.GetExtension(Input);

                switch (extension)
                {
                    case ".json":
                    case ".geojson":
                        return new GeojsonParser();
                    case ".xyz":
                        return new XYZParser();
                    case ".tif":
                    case ".tiff":
                    case ".geotif":
                    case ".geotiff":
                        return new GeoTiffParser();
                    case ".shp":
                        return new ShapefileParser();
                    default:
                        throw new NotSupportedException($"Unknown extension: {extension}");
                }

            }

            switch (InputType)
            {
                case InputType.GeoJson:
                    return new GeojsonParser();
                case InputType.XYZ:
                    return new XYZParser();
                case InputType.Raster:
                    return new GeoTiffParser();
                case InputType.Shapefile:
                    return new ShapefileParser();
                default:
                    throw new NotSupportedException($"{InputType} not supported");
            }
        }

        public IHexagonDataExporter GetResultExporter()
        {
            switch (OutputType)
            {
                case OutputType.None:
                case OutputType.Console:
                    return new ConsoleExporter();
                case OutputType.SQLite:
                    return new SQLiteExporter(Output);
                default:
                    throw new NotSupportedException($"{OutputType} not supported");
            }
        }

        public HexagonDefinition GetHexagonDefinition()
        {
            return new HexagonDefinition(HexagonSize, HexagonReferenceZoom);
        }

    }
    
    internal enum InputType
    {
        None,
        Shapefile,
        Raster,
        XYZ,
        GeoJson
    }
    
    internal enum OutputType
    {
        None,
        Console,
        SQLite
    }
}