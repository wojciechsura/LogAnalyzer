using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class RemovedLastFilteredLogEntryEvent : BaseEvent
    {
        public RemovedLastFilteredLogEntryEvent(int filteredItemIndex, int logEntryIndex)
        {
            FilteredItemIndex = filteredItemIndex;
            LogEntryIndex = logEntryIndex;
        }

        public int FilteredItemIndex { get; }
        public int LogEntryIndex { get; }

        public override string ToString()
        {
            return $"[ ]-[F]-[ ] Removed last filtered log entry event - filteredItemIndex: {FilteredItemIndex}, logEntryIndex: {LogEntryIndex}";
        }
    }
}
