using ConfigurationBase;
using LogAnalyzer.Configuration.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Configuration
{
    public class StoredScript : BaseCollectionItem
    {
        public static readonly string XML_NAME = "StoredScript";

        public StoredScript() : base(XML_NAME)
        { 
            Name = new SimpleValue<string>("Name", this);
            Guid = new GuidValue("Guid", this);
            Filename = new SimpleValue<string>("Filename", this);            
        }

        public SimpleValue<string> Name { get; private set; }
        public GuidValue Guid { get; private set; }
        public SimpleValue<string> Filename { get; private set; }
    }
}
