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
            ProcessingProfiles = new SimpleCollection<ProcessingProfile>("ProcessingProfiles", this, ProcessingProfile.XML_NAME);
            StoredScripts = new SimpleCollection<StoredScript>("StoredScripts", this, StoredScript.XML_NAME);

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
        public SimpleCollection<ProcessingProfile> ProcessingProfiles { get; }
        public SimpleCollection<StoredScript> StoredScripts { get; }
        public OpeningConfiguration OpeningConfiguration { get; }
    }
}
