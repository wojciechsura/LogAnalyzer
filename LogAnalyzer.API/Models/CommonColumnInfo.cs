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

        public LogEntryColumn Column { get; }
        public string Header => Column.GetAttribute<ColumnHeaderAttribute>().Header;
        public string Member => Column.GetAttribute<MemberNameAttribute>().Member;

    }
}
