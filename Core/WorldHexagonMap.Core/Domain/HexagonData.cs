using System;
using System.Collections.Concurrent;
using WorldHexagonMap.Core.Domain.Constants;
using WorldHexagonMap.Core.Domain.Enums;
using Newtonsoft.Json;

namespace WorldHexagonMap.Core.Domain
{
    [Serializable]
    public class HexagonData
    {

        public string Colour
        {
            get
            {
                if (this[HexagonDataType.Colour] == null) return null;
                return this[HexagonDataType.Colour].ToString();
            }
            set { this[HexagonDataType.Colour] = value; }
        }


        public WayMask Road
        {
            get
            {
                if (this[HexagonDataType.Road] == null) return WayMask.None;
                return (WayMask)this[HexagonDataType.Road];
            }
            set
            {
                this[HexagonDataType.Road] = (int)value;
            }
        }

        public WayMask Railway
        {
            get
            {
                if (this[HexagonDataType.Railway] == null) return WayMask.None;
                return (WayMask)this[HexagonDataType.Railway];
            }
            set { this[HexagonDataType.Railway] = (int)value; }
        }

        public WayMask River
        {
            get
            {
                if (this[HexagonDataType.River] == null) return WayMask.None;
                return (WayMask)this[HexagonDataType.River];
            }
            set { this[HexagonDataType.River] = (int)value; }
        }

        public int Land
        {
            get
            {
                if (this[HexagonDataType.Land] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Land].ToString());
            }
            set { this[HexagonDataType.Land] = value; }
        }

        public BoundaryMask LandBoundary
        {
            get
            {
                if (this[HexagonDataType.LandBoundary] == null) return BoundaryMask.None;
                return (BoundaryMask)this[HexagonDataType.LandBoundary];
            }
            set { this[HexagonDataType.LandBoundary] = (int)value; }
        }

        public BoundaryMask Slope
        {
            get
            {
                if (this[HexagonDataType.Slope] == null) return BoundaryMask.None;
                return (BoundaryMask)this[HexagonDataType.Slope];
            }
            set { this[HexagonDataType.Slope] = (int)value; }
        }

        public string Country
        {
            get { return this[HexagonDataType.Country] as string; }
            set { this[HexagonDataType.Country] = value; }
        }

        public string Region
        {
            get { return this[HexagonDataType.Region] as string; }
            set { this[HexagonDataType.Region] = value; }
        }

        public BoundaryMask RegionBoundary
        {
            get
            {
                if (this[HexagonDataType.RegionBoundary] == null) return BoundaryMask.None;
                return (BoundaryMask)this[HexagonDataType.RegionBoundary];
            }
            set { this[HexagonDataType.RegionBoundary] = (int)value; }
        }

        public int Urban
        {
            get
            {
                if (this[HexagonDataType.Urban] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Urban].ToString());
            }
            set { this[HexagonDataType.Urban] = value; }
        }

        public BoundaryMask UrbanBoundary
        {
            get
            {
                if (this[HexagonDataType.UrbanBoundary] == null) return BoundaryMask.None;
                return (BoundaryMask)this[HexagonDataType.UrbanBoundary];
            }
            set { this[HexagonDataType.UrbanBoundary] = (int)value; }
        }

        public int Water
        {
            get
            {
                if (this[HexagonDataType.Water] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Water].ToString());
            }
            set { this[HexagonDataType.Water] = value; }
        }

        public int Forest
        {
            get
            {
                if (this[HexagonDataType.Forest] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Forest].ToString());
            }
            set { this[HexagonDataType.Forest] = value; }
        }

        public BoundaryMask ForestBoundary
        {
            get
            {
                if (this[HexagonDataType.ForestBoundary] == null) return BoundaryMask.None;
                return (BoundaryMask)this[HexagonDataType.ForestBoundary];
            }
            set { this[HexagonDataType.ForestBoundary] = (int)value; }
        }

        public int Altitude
        {
            get
            {
                if (this[HexagonDataType.Altitude] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Altitude].ToString());
            }
            set { this[HexagonDataType.Altitude] = value; }
        }

        public int Level
        {
            get
            {
                if (this[HexagonDataType.Level] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Level].ToString());
            }
            set { this[HexagonDataType.Level] = value; }
        }

        public int Mountain
        {
            get
            {
                if (this[HexagonDataType.Mountain] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Mountain].ToString());
            }
            set { this[HexagonDataType.Mountain] = value; }
        }

        public int Desert
        {
            get
            {
                if (this[HexagonDataType.Desert] == null) return 0;
                return Convert.ToInt32(this[HexagonDataType.Desert].ToString());
            }
            set { this[HexagonDataType.Desert] = value; }
        }


        public HexagonData()
        {
            Data = new ConcurrentDictionary<string, object>();
        }

        [JsonIgnore]
        public ConcurrentDictionary<string, object> Data { get; set; }

        


        public object this[string type]
        {
            get
            {
                object result;
                return Data.TryGetValue(type, out result) ? result : null;
            }
            set
            {
                Data[type] = value;
            }
        }

    }

    public class HexagonDataFieldAttribute : Attribute
    {
    }
}
