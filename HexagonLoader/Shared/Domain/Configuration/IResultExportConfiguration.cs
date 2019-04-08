using WorldHexagonMap.Loader.Domain.Enums;
namespace WorldHexagonMap.Loader.Domain.Configuration
{
    public interface IResultExportConfiguration
    {
        int MinZoom { get; set; }

        int MaxZoom { get; set; }

        TileFormat TileFormat { get; set; }

        bool MergeResults { get; set; }
    }


}
