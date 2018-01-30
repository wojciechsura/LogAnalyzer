using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine.ProcessConditions
{
    public class DateProcessCondition : ProcessCondition
    {
        public DateTime Argument { get; }
    }
}
