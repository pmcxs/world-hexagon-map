using System.IO;
using System.Xml.Serialization;

namespace WorldHexagonMap.Loader.Implementation.Utils
{
    public class XmlUtils
    {

        public static T DeserializeFromFile<T>(string path)
        {
            var s = new XmlSerializer(typeof(T));

            var reader = new StreamReader(path);

            return (T)s.Deserialize(reader);
        }


    }
}
