using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class LogRecord
    {
        public LogRecord(LogEntry logEntry, LogMetadata meta)
        {
            LogEntry = logEntry;
            Meta = meta;
        }

        public LogEntry LogEntry { get; }
        public LogMetadata Meta { get; }
    }
}
