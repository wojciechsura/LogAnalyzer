using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Types.Attributes
{
    public class ColumnHeaderAttribute : Attribute
    {
        public ColumnHeaderAttribute(string header)
        {
            Header = header;
        }

        public string Header { get; }
    }
}
