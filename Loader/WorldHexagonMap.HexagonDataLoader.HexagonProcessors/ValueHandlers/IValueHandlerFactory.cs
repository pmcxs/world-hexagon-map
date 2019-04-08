namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers
{
    public interface IValueHandlerFactory
    {
        IValueHandler GetInstance(string name);
    }
}