﻿
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IHexagonRepository
    {
        bool TryGetValue(HexagonLocationUV hexagonLocationUV, out HexagonData data);
        HexagonData this[HexagonLocationUV hexagonLocationUV] { get; set; }
        Hexagon[] GetHexagons();
        HexagonData GetHexagonData(HexagonLocationUV locationUV);
    }
}