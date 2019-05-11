using System.Collections.Generic;
using System.IO;
using System.Linq;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.Shapefiles.GeoDataParser;
using Xunit;

namespace WorldHexagonMap.HexagonDataLoader.Shapefiles.Test.GeoDataParser
{
    public class ShapefileParserTests
    {
        private readonly List<GeoData> _geoData;

        public ShapefileParserTests()
        {
            var loader = new ShapefileParser();

            _geoData = loader.ParseGeodataFromSource(
                    new LayersConfiguration
                    {
                        Targets = new[]
                        {
                            new LayersLoaderTarget() {Field = "land", Merge = "max"},
                        },
                        Source = Path.Combine("Resources", "sampleAreas.shp")
                    })
                .ToList();
        }

        /// <summary>
        ///     
        /// </summary>
        [Fact]
        public void ParseShapefile_Success()
        {
        }
    }
}