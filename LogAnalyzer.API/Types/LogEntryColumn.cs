using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Types
{
    public enum LogEntryColumn
    {
        [ColumnHeader("Date")]
        [LogRecordMemberName(nameof(LogRecord.LogEntry) + "." + nameof(LogEntry.DisplayDate))]
        Date,
        [ColumnHeader("Severity")]
        [LogRecordMemberName(nameof(LogRecord.LogEntry) + "." + nameof(LogEntry.Severity))]
        Severity,
        [ColumnHeader("Message")]
        [LogRecordMemberName(nameof(LogRecord.LogEntry) + "." + nameof(LogEntry.Message))]
        Message,
        [ColumnHeader("Custom")]
        [LogRecordMemberName(nameof(LogRecord.LogEntry) + "." + nameof(LogEntry.CustomFields))]
        Custom
    }
}
