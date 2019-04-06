using System.Collections.Generic;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Core.Contracts
{
    public interface IDataStoreService
    {

        Task SetPropertiesAsync(string key, params KeyValuePair<string, string>[] properties);

        Task<IList<HexagonData>> GetAllAsync(IList<HexagonLocationUV> hexagonLocations);


    }
}
