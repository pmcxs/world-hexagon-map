using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public class MemoryHexagonRepository : IHexagonRepository
    {
        private readonly IDictionary<HexagonLocationUV, HexagonData> _globalResult =
            new ConcurrentDictionary<HexagonLocationUV, HexagonData>();


        public bool TryGetValue(HexagonLocationUV hexagonLocationUV, out HexagonData data)
        {
            return _globalResult.TryGetValue(hexagonLocationUV, out data);
        }

        public HexagonData this[HexagonLocationUV hexagonLocationUV]
        {
            get => _globalResult[hexagonLocationUV];
            set => _globalResult[hexagonLocationUV] = value;
        }

        public Hexagon[] GetHexagons()
        {
            return _globalResult.Select(kvp => new Hexagon {HexagonData = kvp.Value, LocationUV = kvp.Key}).ToArray();
        }

        public HexagonData GetHexagonData(HexagonLocationUV locationUV)
        {
            return _globalResult.ContainsKey(locationUV) ? _globalResult[locationUV] : new HexagonData();
        }
    }
}