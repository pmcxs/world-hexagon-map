using CommandLine;
using WorldHexagonMap.HexagonDataLoader.Cmd.GeoDataToHexagonData;
using WorldHexagonMap.HexagonDataLoader.Cmd.HexagonDataToMap;

namespace WorldHexagonMap.HexagonDataLoader.Cmd
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GeoDataToHexagonDataArgs, HexagonDataToMapArgs>(args)
                .WithParsed<GeoDataToHexagonDataArgs>(GeoDataToHexagonDataAction.Process)
                .WithParsed<HexagonDataToMapArgs>(HexagonDataToMapAction.Process);
            
        }
        
    }
}