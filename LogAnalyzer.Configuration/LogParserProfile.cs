using System;
using System.Xml.Serialization;

namespace LogAnalyzer.Configuration
{
    public class LogParserProfile
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Guid")]
        public Guid Guid { get; set; }
        [XmlElement("ParserUniqueName")]
        public string ParserUniqueName { get; set; }
        [XmlElement("SerializedProfile")]
        public string SerializedProfile { get; set; }
    }
}