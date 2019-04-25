namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public interface IValueHandlerFactory
    {
        IValueHandler GetInstance(string name);
    }
}