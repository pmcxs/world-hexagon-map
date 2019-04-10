using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class PixelProcessor : IHexagonProcessor
    {
        private readonly HexagonDefinition _hexagonDefinition;
        private readonly IHexagonService _hexagonService;

        public PixelProcessor(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IEnumerable<HexagonProcessorResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null)
        {
            foreach (var coordinates in geoData.Points)
                if (coordinates.Length == 1)
                    //Single point mode 
                {
                    var hexagonLocation =
                        _hexagonService.GetHexagonLocationUVForPointXY(coordinates[0], _hexagonDefinition);

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
                        _hexagonService.GetHexagonsInsideBoundingBox(coordinates[0], coordinates[1], _hexagonDefinition);

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