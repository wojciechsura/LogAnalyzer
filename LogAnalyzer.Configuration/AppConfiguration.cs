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

        public AppConfiguration() 
            : base(XML_NAME)
        {
            Session = new Session(this);

            LogParserProfiles = new SimpleCollection<LogParserProfile>("LogParserProfiles", this, LogParserProfile.XML_NAME);

            OpeningConfiguration = new OpeningConfiguration(this);
        }

        public void Defaults()
        {
            this.SetDefaults();
            LogParserProfiles.Clear();
            LogParserProfiles.Add(new LogParserProfile("Parse whole line as message", Guid.NewGuid(), "LogParser.Line", ""));
        }

        public Session Session { get; }

        public SimpleCollection<LogParserProfile> LogParserProfiles { get; }

        public OpeningConfiguration OpeningConfiguration { get; }
    }
}
