namespace WorldHexagonMap.Loader.ResultExporters
{
    public interface IResultExporterFactory
    {
        IResultExporter GetInstance(string resultExporter);
    }
}