using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.Domain.Configuration
{
    public interface ITileExporterConfiguration
    {
        int MinZoom { get; set; }
        int MaxZoom { get; set; }

        int TileSize { get; set; }
        TileFormat TileFormat { get; set; }
    }
}
