using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine.PredicateDescriptions
{
    public abstract class StringPredicateDescription : PredicateDescription
    {
        public string Argument { get; set; }
        public bool CaseSensitive { get; set; }
    }
}
