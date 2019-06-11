using MapGenerator.Core.Common;

namespace MapGenerator.Core.Contracts
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}