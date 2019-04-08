using WorldHexagonMap.HexagonDataLoader.ResultExporters;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp.Configuration
{
    public interface IResultExportConfiguration
    {
        int MinZoom { get; set; }

        int MaxZoom { get; set; }

        TileFormat TileFormat { get; set; }

        bool MergeResults { get; set; }
    }
}