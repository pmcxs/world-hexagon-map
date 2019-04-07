using System.Collections.Generic;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;
using WorldHexagonMap.Loader.Domain.Delegates;
using WorldHexagonMap.Loader.ValueHandlers;

namespace WorldHexagonMap.Loader.HexagonParsers
{
    public class PixelParser : IHexagonParser
    {
        private readonly IHexagonService _hexagonService;
        private readonly HexagonDefinition _hexagonDefinition;

        public PixelParser(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
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
