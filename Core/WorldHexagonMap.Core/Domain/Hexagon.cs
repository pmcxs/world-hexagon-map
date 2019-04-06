using System;
using System.Collections.Generic;

namespace WorldHexagonMap.Core.Domain
{
    /// <summary>
    /// Represents a regular hexagon
    /// </summary>
    [Serializable]
    public class Hexagon : IEqualityComparer<Hexagon>
    {
        public HexagonLocationUV LocationUV { get; set; }

        public HexagonData HexagonData { get; set; }

        public override int GetHashCode()
        {
            return LocationUV.GetHashCode();
        }

        protected bool Equals(Hexagon other)
        {
            return LocationUV.Equals(other.LocationUV);
        }

        public string GetHexagonKey()
        {
            return LocationUV.U + "_" + LocationUV.V;
        }

        public void MergeFrom(Hexagon hexagon)
        {
            HexagonData.LandBoundary &= hexagon.HexagonData.LandBoundary;
            HexagonData.ForestBoundary &= hexagon.HexagonData.ForestBoundary;
            HexagonData.UrbanBoundary &= hexagon.HexagonData.UrbanBoundary;
            HexagonData.Road |= hexagon.HexagonData.Road;
            HexagonData.River |= hexagon.HexagonData.River;

            HexagonData.Land = Math.Max(HexagonData.Land, hexagon.HexagonData.Land);
            HexagonData.Forest = Math.Max(HexagonData.Forest, hexagon.HexagonData.Forest);
            HexagonData.Water = Math.Max(HexagonData.Water, hexagon.HexagonData.Water);
            HexagonData.Urban = Math.Max(HexagonData.Urban, hexagon.HexagonData.Urban);
            HexagonData.Altitude = Math.Max(HexagonData.Altitude, hexagon.HexagonData.Altitude);

            //foreach (KeyValuePair<string, object> data in hexagon.HexagonData.Data)
            //{

            //    if (data.Value == null) continue;

            //    switch (data.Key)
            //    {
            //        case HexagonDataType.LandBoundary:
            //            HexagonData[data.Key] = (BoundaryMask)(HexagonData[data.Key] ?? BoundaryMask.None) & (BoundaryMask)data.Value;
            //            break;

            //        case HexagonDataType.River:
            //        case HexagonDataType.Road:
            //            HexagonData[data.Key] = (WayMask)(HexagonData[data.Key] ?? WayMask.None) | (WayMask)data.Value;
            //            break;

            //        default:
            //            HexagonData[data.Key] = HexagonData[data.Key] ?? data.Value;
            //            break;
            //    }
            //}
        }

        public bool Equals(Hexagon x, Hexagon y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Hexagon obj)
        {
            return obj.GetHashCode();
        }

    }
}
