using System.Collections.Generic;

namespace HexagonDataLoader.Core.Common
{
    public class GeoData
    {
        public IDictionary<string, object> Values { get; set; }

        public PointXY[][] Points { get; set; }

        public DataType DataType { get; set; }
    }
}