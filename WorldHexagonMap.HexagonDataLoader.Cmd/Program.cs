using CommandLine;
using WorldHexagonMap.HexagonDataLoader.Cmd.GeoDataToHexagonData;
using WorldHexagonMap.HexagonDataLoader.Cmd.HexagonDataToMap;

namespace WorldHexagonMap.HexagonDataLoader.Cmd
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GeoDataToHexagonDataOptions, HexagonDataToMapOptions>(args)
                .WithParsed<GeoDataToHexagonDataOptions>(GeoDataToHexagonDataAction.Process)
                .WithParsed<HexagonDataToMapOptions>(HexagonDataToMapAction.Process);
            
        }
        
    }
}