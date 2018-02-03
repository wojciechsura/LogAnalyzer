using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;

namespace LogAnalyzer.Models.DialogResults
{
    public class FindResult
    {
        public FindAction Action { get; set; }
        public PredicateDescription FindCondition { get; set; }
    }
}
