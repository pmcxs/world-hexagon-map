using CommandLine;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.MapTileGenerator.ConsoleApp
{
    internal class MapTileGeneratorOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('t', "tile", HelpText = "Time in the format z:x:y. Ex: --tile 10:343:343. Supplying this parameter overrides the boundaries parameters (n,s,e,w)")]
        public string Tile { get; set; }
        
        [Option("hexagonsize", Default = 10)]
        public double HexagonSize { get; set; }

        [Option("referencezoom", Default = 10)]
        public int HexagonReferenceZoom { get; set; }

        [Option('i', "input", HelpText = "Input file that contains the hexagon data to be processed")]
        public string Input { get; set; }

        public InputType InputType { get; set; }
        
        [Option('o',"output")]
        public string Output { get; set; }

        [Option('w',"west", HelpText = "Minimum Longitude - West boundary")]
        public string West { get; set; }
        
        [Option('e',"east", HelpText = "Maximum Longitude - East boundary")]
        public string East { get; set; }
        
        [Option('n',"north", HelpText = "Maximum Latitude - North boundary")]
        public string North { get; set; }
        
        [Option('s',"south", HelpText = "Minimum Latitude - South boundary")]
        public string South { get; set; }

        [Option("outputtype",Default = OutputType.Geojson)]
        public OutputType OutputType { get; set; }
       
        public HexagonDefinition GetHexagonDefinition()
        {
            return new HexagonDefinition(HexagonSize);
        }
    }

    internal enum InputType
    {
        Sqlite
    }

    internal enum OutputType
    {
        Console,
        Geojson,
        Shapefile,
        VectorTile
    }
}