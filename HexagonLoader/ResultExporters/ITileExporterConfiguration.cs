namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public interface ITileExporterConfiguration
    {
        int MinZoom { get; set; }
        int MaxZoom { get; set; }

        int TileSize { get; set; }
        TileFormat TileFormat { get; set; }
    }
}