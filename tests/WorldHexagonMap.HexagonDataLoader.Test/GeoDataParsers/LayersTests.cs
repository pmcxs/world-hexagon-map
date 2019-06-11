using System.IO;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using Xunit;

namespace WorldHexagonMap.HexagonDataLoader.Test.GeoDataParsers
{
    public class LayersTests
    {
        [Fact]
        public void ReadSimpleVersion_Success()
        {
            Layers configuration = XmlUtils.DeserializeFromFile<Layers>(
                Path.Combine("Resources", "LayersConfiguration_simple.xml"));
            
            Assert.Equal(2, configuration.Sources.Length);
            Assert.Null(configuration.Sources[0].Filters);
            Assert.Equal(3, configuration.Sources[0].Targets.Length);
            Assert.Single(configuration.Sources[1].Filters);
            Assert.Single(configuration.Sources[1].Targets);

        }
    }
}


