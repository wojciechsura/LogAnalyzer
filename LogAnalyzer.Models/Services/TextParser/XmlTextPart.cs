using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogAnalyzer.Models.Services.TextParser
{
    public class XmlTextPart : BaseTextPart
    {
        public XmlTextPart(XmlDocument document)
        {
            Document = document;
        }

        public XmlDocument Document { get; }
    }
}
