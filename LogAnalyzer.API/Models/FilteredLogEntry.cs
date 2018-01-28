using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class FilteredLogEntry
    {
        public FilteredLogEntry(int logEntryIndex, 
            DateTime date, 
            string severity, 
            string message)
        {
            LogEntryIndex = logEntryIndex;
            Date = date;
            Severity = severity;
            Message = message;
        }

        public int LogEntryIndex { get; }

        public DateTime Date { get; }
        public string Severity { get; }
        public string Message { get; }
    }
}
