using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class GeoData
    {
        public IDictionary<string, object> Values { get; set; }

        public PointXY[][] Points { get; set; }

        public DataType DataType { get; set; }
    }
}