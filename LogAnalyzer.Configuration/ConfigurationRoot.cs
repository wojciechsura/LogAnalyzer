using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogAnalyzer.Configuration
{
    [XmlRoot("Config")]
    public class ConfigurationRoot
    {
        [XmlArray("LogParserProfiles"), XmlArrayItem(typeof(LogParserProfile), ElementName = "LogParserProfile")]
        public List<LogParserProfile> LogParserProfiles { get; set; }

        public ConfigurationRoot()
        {
            LogParserProfiles = new List<LogParserProfile>
            {
                new LogParserProfile
                {
                    Name = "Parse whole line as message",
                    ParserUniqueName = "LogParser.Line",
                    SerializedProfile = ""
                }
            };
        }
    }
}
