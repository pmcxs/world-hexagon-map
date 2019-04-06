using WorldHexagonMap.Loader.Domain.Configuration;
using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.Implementation.Configuration
{
    public class ResultExportConfiguration : IResultExportConfiguration
    {
        public int MinZoom { get; set; }

        public int MaxZoom { get; set; }

        public TileFormat TileFormat { get; set; }

        public bool MergeResults { get; set; }



    }


}
