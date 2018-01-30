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
            string message,
            IReadOnlyList<string> customFields)
        {
            LogEntryIndex = logEntryIndex;
            Date = date;
            Severity = severity;
            Message = message;
            CustomFields = customFields;
        }

        public int LogEntryIndex { get; }

        public DateTime Date { get; }
        public string DisplayDate => Date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public string Severity { get; }
        public string Message { get; }
        public IReadOnlyList<string> CustomFields { get; }
    }
}
