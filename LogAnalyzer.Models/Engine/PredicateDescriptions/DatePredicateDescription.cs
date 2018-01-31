using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;

namespace LogAnalyzer.Models.Engine.PredicateDescriptions
{
    public class DatePredicateDescription : PredicateDescription
    {
        public DateTime Argument { get; set; }
    }
}
