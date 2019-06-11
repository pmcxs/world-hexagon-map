using System.IO;
using System.Xml.Serialization;

namespace HexagonDataLoader.Core.Utils
{
    public static class XmlUtils
    {
        public static T DeserializeFromFile<T>(string path)
        {
            var reader = new StreamReader(path);
            return DeserializeFromStream<T>(reader);
        }

        public static T DeserializeFromStream<T>(StreamReader reader)
        {
            var s = new XmlSerializer(typeof(T));
            return (T)s.Deserialize(reader);
        }
    }
}