using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class RemovedLastFilteredLogEntryEvent : BaseEvent
    {
        public RemovedLastFilteredLogEntryEvent(int index)
        {
            Index = index;
        }

        public override string ToString()
        {
            return $"Removed last filtered log entry event - index: {Index}";
        }

        public int Index { get; }
    }
}
