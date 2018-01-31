using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Types
{
    public class SummaryDisplayAttribute : Attribute
    {
        public SummaryDisplayAttribute(string summary)
        {
            Summary = summary;
        }

        public string Summary { get; }
    }
}
