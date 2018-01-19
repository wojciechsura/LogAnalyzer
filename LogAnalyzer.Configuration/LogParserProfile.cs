using System.Xml.Serialization;

namespace LogAnalyzer.Configuration
{
    public class LogParserProfile
    {
        [XmlElement("ParserUniqueName")]
        public string ParserUniqueName { get; set; }
        [XmlElement("SerializedProfile")]
        public string SerializedProfile { get; set; }
    }
}