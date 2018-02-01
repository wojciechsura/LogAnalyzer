using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class FilterEntry
    {
        public FilterEntry()
        {

        }

        public PredicateDescription PredicateDescription { get; set; }
        public FilterAction Action { get; set; }
    }
}
