using System.Collections.Generic;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers
{
    public class PixelProcessor : IHexagonProcessor
    {
        private readonly IHexagonService _hexagonService;
        private readonly HexagonDefinition _hexagonDefinition;

        public PixelProcessor(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null)
        {
            foreach (PointXY[] coordinates in geoData.Points)
            {

                if (coordinates.Length == 1)    
                //Single point mode 
                {
                    var hexagonLocation = _hexagonService.GetHexagonLocationUVForPointXY(coordinates[0], _hexagonDefinition);

                    yield return new HexagonLoaderResult
                    {
                        HexagonLocationUV = hexagonLocation,
                        Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                    };
                }
                else
                //Square mode
                {
                    IList<HexagonLocationUV> hexagons = _hexagonService.GetHexagonsInsideBoudingBox(coordinates[0], coordinates[1], _hexagonDefinition);

                    foreach (var hexagonLocation in hexagons)
                    {
                        yield return new HexagonLoaderResult
                        {
                            HexagonLocationUV = hexagonLocation,
                            Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                        };
                    }
                    
                }
                

            }

        }


    }
}
