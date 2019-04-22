using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class PixelProcessor : IHexagonProcessor
    {
        public IEnumerable<HexagonProcessorResult> ProcessGeoData(GeoData geoData, HexagonDefinition hexagonDefinition, IValueHandler valueHandler = null)
        {
            foreach (var coordinates in geoData.Points)
                if (coordinates.Length == 1)
                    //Single point mode 
                {
                    var hexagonLocation =
                        HexagonService.GetHexagonLocationUVForPointXY(coordinates[0], hexagonDefinition);

                    yield return new HexagonProcessorResult
                    {
                        HexagonLocationUV = hexagonLocation,
                        Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                    };
                }
                else
                    //Square mode
                {
                    var hexagons =
                        HexagonService.GetHexagonsInsideBoundingBox(coordinates[0], coordinates[1], hexagonDefinition);

                    foreach (var hexagonLocation in hexagons)
                        yield return new HexagonProcessorResult
                        {
                            HexagonLocationUV = hexagonLocation,
                            Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                        };
                }
        }
    }
}