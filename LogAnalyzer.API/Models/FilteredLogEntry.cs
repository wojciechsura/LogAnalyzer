using LogAnalyzer.API.Models;
using LogAnalyzer.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class FilteredLogEntry : LogEntry
    {
        public FilteredLogEntry(int logEntryIndex, 
            DateTime date, 
            string severity, 
            string message,
            IReadOnlyList<string> customFields)
            : base(date, severity, message, customFields)
        {
            LogEntryIndex = logEntryIndex;
        }

        public int LogEntryIndex { get; }
    }
}
