using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.ValueHandlers
{
    public interface IValueHandlerFactory
    {
        IValueHandler GetInstance(string name);
    }
}