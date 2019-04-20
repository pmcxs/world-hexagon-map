using System;

namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public class ResultExporterFactory : IResultExporterFactory
    {
        public IResultExporter GetInstance(string name)
        {
            //TODO: Plugable logic instead of having this hardcoded?
            switch (name.ToLower())
            {
                case "console":
                    return new ConsoleExporter();
                
                case "sqlite":
                    return new SQLiteExporter();
                
                default:
                    throw new NotSupportedException($"Type {name} does not exist");
                
            }
        }
    }
}