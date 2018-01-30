using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationBase;
using LogAnalyzer.Configuration.Base;

namespace LogAnalyzer.Configuration
{
    public class Session : BaseItem
    {
        public static readonly string XML_NAME = "Session";

        public Session(BaseItemContainer parent) 
            : base(XML_NAME, parent)
        {
            LastParserProfile = new GuidValue("LastParserProfile", this, Guid.Empty);
        }

        public GuidValue LastParserProfile { get; }
    }
}
