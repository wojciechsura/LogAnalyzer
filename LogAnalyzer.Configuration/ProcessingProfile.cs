using ConfigurationBase;
using LogAnalyzer.Configuration.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Configuration
{
    public class ProcessingProfile : BaseCollectionItem
    {
        public static readonly string XML_NAME = "ProcessingProfile";

        public ProcessingProfile() : base(XML_NAME)
        {
            Name = new SimpleValue<string>("Name", this);
            Guid = new GuidValue("Guid", this);
            FilteringSettings = new SimpleValue<string>("FilteringSettings", this);
            HighlightingSettings = new SimpleValue<string>("HighlightingSettings", this);
        }

        public SimpleValue<string> Name { get; private set; }
        public GuidValue Guid { get; private set; }
        public SimpleValue<string> FilteringSettings { get; private set; }
        public SimpleValue<string> HighlightingSettings { get; private set; }
    }
}
