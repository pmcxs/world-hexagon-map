using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.HexagonDataLoader.Domain
{
    public class HexagonLoaderResult
    {
        public object Value;
        public HexagonLocationUV HexagonLocationUV { get; set; }

        public string Target { get; set; }
        public MergeStrategy MergeStrategy { get; set; }
    }
}