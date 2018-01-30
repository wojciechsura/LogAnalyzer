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
        [MemberName(nameof(FilteredLogEntry.DisplayDate))]
        Date,
        [ColumnHeader("Severity")]
        [MemberName(nameof(FilteredLogEntry.Severity))]
        Severity,
        [ColumnHeader("Message")]
        [MemberName(nameof(FilteredLogEntry.Message))]
        Message,
        [ColumnHeader("Custom")]
        [MemberName(nameof(FilteredLogEntry.CustomFields))]
        Custom
    }
}
