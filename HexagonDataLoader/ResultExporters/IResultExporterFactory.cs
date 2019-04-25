namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public interface IResultExporterFactory
    {
        IResultExporter GetInstance(string resultExporter);
    }
}