using HexagonDataLoader.Core.Common;

namespace HexagonDataLoader.Core.Contracts
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}