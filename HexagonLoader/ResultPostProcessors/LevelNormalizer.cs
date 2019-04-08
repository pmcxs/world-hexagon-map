//using System.Composition;

using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultPostProcessors
{
    //[Export("postprocessor_handler_level_normalizer", typeof(IPostProcessor))]
    public class LevelNormalizer : IPostProcessor
    {
        public void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {
            if (hexagon.HexagonData.Land == 0 || hexagon.HexagonData.Water > 0)
            {
                hexagon.HexagonData.Level = 0;
                return;
            }

            var altitude = hexagon.HexagonData.Altitude;

            if (altitude <= 5)
                hexagon.HexagonData.Level = 0;
            else if (altitude < 10)
                hexagon.HexagonData.Level = 1;
            else if (altitude < 20)
                hexagon.HexagonData.Level = 2;
            else if (altitude < 30)
                hexagon.HexagonData.Level = 3;
            else if (altitude < 40)
                hexagon.HexagonData.Level = 4;
            else
                hexagon.HexagonData.Level = 5;
        }
    }
}