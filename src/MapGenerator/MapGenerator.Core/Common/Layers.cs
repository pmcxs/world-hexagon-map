using System.Xml.Serialization;

namespace MapGenerator.Core.Common
{
    /// <remarks />
    [XmlType("layers")]
    public class Layers
    {
        /// <remarks />
        [XmlArray("sources"), XmlArrayItem("loader")]
        public LayersConfiguration[] Sources { get; set; }

        /// <remarks />
        [XmlArray("post-processors"), XmlArrayItem("post-processor")]
        public LayersPostprocessor[] PostProcessors { get; set; }

    }

    /// <remarks />
    [XmlType]
    public class LayersConfiguration
    {
        /// <remarks />
        [XmlArray("filters"), XmlArrayItem("filter")]
        public LayersLoaderFilter[] Filters { get; set; }

        /// <remarks />
        [XmlArray("targets"), XmlArrayItem("target")]
        public LayersLoaderTarget[] Targets { get; set; }

        /// <remarks />
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <remarks />
        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("interpolate")]
        public bool Interpolate { get; set; }

        /// <remarks />
        [XmlAttribute("xoffset")]
        public decimal XOffset { get; set; }

        /// <remarks />
        [XmlAttribute("yoffset")]
        public decimal YOffset { get; set; }

    }

    /// <remarks />
    [XmlType]
    public class LayersLoaderFilter
    {
        /// <remarks />
        [XmlAttribute("field")]
        public string Field { get; set; }

        /// <remarks />
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    /// <remarks />
    [XmlType]
    public class LayersLoaderTarget
    {
        /// <remarks />
        [XmlAttribute("field")]
        public string Field { get; set; }

        /// <remarks />
        [XmlAttribute("source")]
        public string SourceField { get; set; }

        /// <remarks />
        [XmlAttribute("destination")]
        public string Destination { get; set; }

        /// <remarks />
        [XmlAttribute("handler")]
        public string Handler { get; set; }

        [XmlAttribute("merge")]
        public string Merge { get; set; }
    }

    /// <remarks />
    [XmlType]
    public class LayersPostprocessor
    {
        /// <remarks />
        [XmlAttribute("handler")]
        public string Handler { get; set; }

        /// <remarks />
        [XmlAttribute("iterations")]
        public byte Iterations { get; set; }
    }
}