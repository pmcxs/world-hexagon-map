using System.Collections;
using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class GeoData
    {
        public string Name { get; set; }
        
        public IDictionary<string, object> Values { get; set; }

        public PointXY[][] Points { get; set; }

        public DataType DataType { get; set; }
    }


    public class GeoDataCollection : ICollection<GeoData>
    {
        private ICollection<GeoData> GeoData { get; set; }

        public string SourceProjection { get; set; }


        public void Add(GeoData item)
        {
            GeoData.Add(item);
        }

        public void Clear()
        {
            GeoData.Clear();
        }

        public bool Contains(GeoData item)
        {
            return GeoData.Contains(item);
        }

        public void CopyTo(GeoData[] array, int arrayIndex)
        {
            GeoData.CopyTo(array, arrayIndex);
        }

        public int Count => GeoData.Count;

        public bool IsReadOnly => GeoData.IsReadOnly;

        public bool Remove(GeoData item)
        {
            return GeoData.Remove(item);
        }

        public IEnumerator<GeoData> GetEnumerator()
        {
            return GeoData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GeoData.GetEnumerator();
        }
    }
}