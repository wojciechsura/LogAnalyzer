using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class RemovedLastHighlightedLogEntryEvent : BaseEvent
    {
        public RemovedLastHighlightedLogEntryEvent(int filteredLogEntryIndex, int logEntryIndex)
        {
            this.FilteredLogEntryIndex = filteredLogEntryIndex;
            this.LogEntryIndex = logEntryIndex;
        }

        public override string ToString()
        {
            return $"[ ]-[ ]-[H] Removed last highlighted log entry event - logEntry: {LogEntryIndex}, filteredLogEntry: {FilteredLogEntryIndex}";
        }

        public int FilteredLogEntryIndex { get; }
        public int LogEntryIndex { get; }
    }
}
