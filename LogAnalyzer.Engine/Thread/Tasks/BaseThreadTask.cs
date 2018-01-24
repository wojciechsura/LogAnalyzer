using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Thread.Tasks
{
    abstract class BaseThreadTask
    {
        public abstract int Priority { get; }
    }
}
