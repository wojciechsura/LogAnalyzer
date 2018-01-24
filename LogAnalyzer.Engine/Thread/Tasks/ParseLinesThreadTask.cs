using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Thread.Tasks
{
    class ParseLinesThreadTask : BaseThreadTask
    {
        private static readonly int TASK_PRIORITY = 1;
       
        public override int Priority => TASK_PRIORITY;
    }
}
