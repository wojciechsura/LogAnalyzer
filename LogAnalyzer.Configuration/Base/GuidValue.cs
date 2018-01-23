using ConfigurationBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogAnalyzer.Configuration.Base
{
    public class GuidValue : BaseTypedValue<Guid>
    {
        protected override Guid DeserializeValue(string text)
        {
            return Guid.Parse(text);
        }

        protected override string SerializeValue(Guid value)
        {
            return value.ToString();
        }

        public GuidValue(string xmlName, 
            BaseItemContainer owner,
            Guid defaultValue = default(Guid),
            XmlStoragePlace xmlStoragePlace = XmlStoragePlace.Subnode) : base(xmlName, owner, defaultValue, xmlStoragePlace)
        {
        }
    }
}
