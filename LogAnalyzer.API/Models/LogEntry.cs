using LogAnalyzer.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    // For thread-safety this object must stay read-only
    public class LogEntry : BaseLogEntry
    {
        private readonly ILogEntryMetaHandler handler;

        public LogEntry(BaseLogEntry source, int index, ILogEntryMetaHandler handler)
            : base(source.Date, source.Severity, source.Message, source.CustomFields)
        {
            Index = index;
            this.handler = handler;
        }

        public int Index { get; }
    }
}
