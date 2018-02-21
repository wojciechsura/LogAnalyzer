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
    public class CustomColumnInfo : BaseColumnInfo
    {
        private int index;
        private string name;

        public CustomColumnInfo(int index, string name)
        {
            this.index = index;
            this.name = name;
        }

        public override string GetStringValue(LogEntry logEntry)
        {
            return logEntry.CustomFields[index];
        }

        public override string Header => name;
        public override string Member => $"{LogEntryColumn.Custom.GetAttribute<LogRecordMemberNameAttribute>().Member}[{index}]";
        public int Index => index;
        public string Name => name;
    }
}
