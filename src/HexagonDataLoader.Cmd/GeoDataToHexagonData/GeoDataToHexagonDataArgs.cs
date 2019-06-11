using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Common.Enums;
using HexagonDataLoader.Core.Contracts;
using HexagonDataLoader.GeoDataParsers;
using HexagonDataLoader.HexagonDataExporters;

namespace HexagonDataLoader.Cmd.GeoDataToHexagonData
{
    [Verb("geo2hexagon", HelpText = "Reads geographical data and generates hexagonal information")]
    internal class GeoDataToHexagonDataArgs
    {
        [Option('v', "verbose", HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "input", HelpText = "Path to the data-source being imported. Might require specifying the 'inputtype' as well")]
        public string Input { get; set; }

        [Option(longName: "inputtype", HelpText = "Type of the 'input' source. Supported formats: Shapefile, Raster, XYZ, Geojson")]
        public InputType InputType { get; set; }

        [Option('t', "targets", HelpText = @"
            Defines the actual properties that will be set on the hexagons, including their names and source values.
            Format: 
                <destination property>:<source property>
                <destination property>
                <destination property:handler(<name of the hander>)
            Examples:
                country:iso_a2      (will read from field 'iso_a2' in the source file and create a 'country' property
                river               (will create property 'river'. Typically used for rasters, where there aren't source fields
                altitude:handler(wikimedia_altitude)  (will create property 'altitude', using a calculation called 'wikimedia_altitude')")]
        public IEnumerable<string> Targets { get; set; }

        [Option('f', "filters", HelpText = "Filters the data being processed")]
        public IEnumerable<string> Filters { get; set; }

        [Option('m', "merge", Required = false, Default = MergeStrategy.Replace, HelpText = "Defines how a new value should be merged with an existing one for the same hexagon")]
        public MergeStrategy Merge { get; set; }

        [Option('o', "output", Required = false)]
        public string Output { get; set; }

        [Option('h', "hexagonsize", Default = 10, HelpText = "The size of the hexagon side on the reference zoom. Total size = 256 << referencezoom")]
        public double HexagonSize { get; set; }

        [Option("referencezoom", Default = 10, HelpText = "The zoom level at which the size calculations are made")]
        public int HexagonReferenceZoom { get; set; }

        [Option("outputtype", Required = false, Default = OutputType.Console)]
        public OutputType OutputType { get; set; }

        [Option("interpolate", Default = true, HelpText = "Only relevant for rasters, will treat the pixels as geo rectangles")]
        public bool Interpolate { get; set; }

        [Option('w', "west", HelpText = "Minimum Longitude - West boundary")]
        public string West { get; set; }

        [Option('e', "east", HelpText = "Maximum Longitude - East boundary")]
        public string East { get; set; }

        [Option('n', "north", HelpText = "Maximum Latitude - North boundary")]
        public string North { get; set; }

        [Option('s', "south", HelpText = "Minimum Latitude - South boundary")]
        public string South { get; set; }

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

                filters.Add(new LayersLoaderFilter { Field = lines[0], Value = lines[1] });

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
        GeoJson,
        Manifest
    }

    internal enum OutputType
    {
        None,
        Console,
        SQLite
    }
}