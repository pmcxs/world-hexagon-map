using CommandLine;
using HexagonDataLoader.Cmd.GeoDataToHexagonData;
using HexagonDataLoader.Cmd.HexagonDataToMap;

namespace HexagonDataLoader.Cmd
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