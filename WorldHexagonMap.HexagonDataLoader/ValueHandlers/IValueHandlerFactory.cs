namespace WorldHexagonMap.HexagonDataLoader.ValueHandlers
{
    public interface IValueHandlerFactory
    {
        IValueHandler GetInstance(string name);

        IValueHandler GetInstance<T>() where T : IValueHandler;

        void RegisterImplementation(string extension, IValueHandler handler);
    }
}