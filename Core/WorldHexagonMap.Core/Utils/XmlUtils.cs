using System.IO;
using System.Xml.Serialization;

namespace WorldHexagonMap.Core.Utils
{
    public static class XmlUtils
    {
        public static T DeserializeFromFile<T>(string path)
        {
            var s = new XmlSerializer(typeof(T));

            var reader = new StreamReader(path);

            return (T) s.Deserialize(reader);
        }
    }
}