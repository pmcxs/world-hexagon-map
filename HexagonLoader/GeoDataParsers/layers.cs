using System.Xml.Serialization;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class layers
    {
        /// <remarks />
        [XmlArrayItem("loader", IsNullable = false)]
        public layersLoader[] sources { get; set; }

        /// <remarks />
        [XmlArray("post-processors"), XmlArrayItem("post-processor", IsNullable = false)]
        public layersPostprocessor[] postprocessors { get; set; }

    }

    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class layersLoader
    {
        /// <remarks />
        [XmlArrayItem("filter", IsNullable = false)]
        public layersLoaderFilter[] filters { get; set; }

        /// <remarks />
        [XmlArrayItem("target", IsNullable = false)]
        public layersLoaderTarget[] targets { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string type { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string source { get; set; }

        [XmlAttribute]
        public bool interpolate { get; set;  }

        [XmlAttribute]
        public bool interpolateSpecified { get; set; }

        /// <remarks />
        [XmlAttribute]
        public decimal xoffset { get; set; }

        /// <remarks />
        [XmlIgnore]
        public bool xoffsetSpecified { get; set; }

        /// <remarks />
        [XmlAttribute]
        public decimal yoffset { get; set; }

        /// <remarks />
        [XmlIgnore]
        public bool yoffsetSpecified { get; set; }
    }

    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class layersLoaderFilter
    {
        /// <remarks />
        [XmlAttribute]
        public string field { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string value { get; set; }
    }

    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class layersLoaderTarget
    {
        /// <remarks />
        [XmlAttribute]
        public string field { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string handler { get; set; }

        [XmlAttribute]
        public string merge { get; set; }
    }

    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class layersPostprocessor
    {
        /// <remarks />
        [XmlAttribute]
        public string handler { get; set; }

        /// <remarks />
        [XmlAttribute]
        public byte iterations { get; set; }
    }
}