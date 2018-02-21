using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class CommonColumnInfo : BaseColumnInfo
    {
        public CommonColumnInfo(LogEntryColumn column)
        {
            if (column == LogEntryColumn.Custom)
                throw new ArgumentException("Custom column cannot be passed to CommonColumnInfo!");
            Column = column;
        }

        public override string GetStringValue(LogEntry logEntry)
        {
            switch (Column)
            {
                case LogEntryColumn.Date:
                    return logEntry.DisplayDate;
                case LogEntryColumn.Message:
                    return logEntry.Message;
                case LogEntryColumn.Severity:
                    return logEntry.Severity;
                default:
                    throw new InvalidOperationException("Invalid column type!");
            }
        }

        public LogEntryColumn Column { get; }
        public override string Header => Column.GetAttribute<ColumnHeaderAttribute>().Header;
        public override string Member => Column.GetAttribute<LogRecordMemberNameAttribute>().Member;

    }
}
