using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    // For thread-safety this object must stay read-only
    public class LogEntry
    {
        public LogEntry(DateTime date, string severity, string message, IReadOnlyList<string> customFields)
        {
            Date = date;
            Severity = severity;
            Message = message;
            CustomFields = customFields;
        }

        public DateTime Date { get; }
        public string Severity { get; }
        public string Message { get; }
        public IReadOnlyList<string> CustomFields { get; }
    }
}
