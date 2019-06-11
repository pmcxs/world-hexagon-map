using HexagonDataLoader.Core.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace HexagonDataLoader.GeoDataParsers.Test
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