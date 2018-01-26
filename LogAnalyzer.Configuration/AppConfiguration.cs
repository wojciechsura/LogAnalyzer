using ConfigurationBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Configuration
{
    public class AppConfiguration : BaseConfiguration
    {
        public static readonly string XML_NAME = "Config";

        private SimpleCollection<LogParserProfile> logParserProfiles;

        public AppConfiguration() 
            : base(XML_NAME)
        {
            logParserProfiles = new SimpleCollection<LogParserProfile>("LogParserProfiles", this, LogParserProfile.XML_NAME);
        }

        public void Defaults()
        {
            this.SetDefaults();
            logParserProfiles.Add(new LogParserProfile("Parse whole line as message", Guid.NewGuid(), "LogParser.Line", ""));
        }

        public SimpleCollection<LogParserProfile> LogParserProfiles => logParserProfiles;
    }
}
