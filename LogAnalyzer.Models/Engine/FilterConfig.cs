using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class FilterConfig
    {
        public FilterAction DefaultAction { get; set; }
        public List<FilterEntry> FilterEntries { get; set; }
    }
}
