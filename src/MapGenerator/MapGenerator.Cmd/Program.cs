using CommandLine;
using MapGenerator.Cmd.GeoDataToHexagonData;
using MapGenerator.Cmd.HexagonDataToMap;

namespace MapGenerator.Cmd
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