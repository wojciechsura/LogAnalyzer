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
        private string header;

        public CustomColumnInfo(int index, string header)
        {
            this.index = index;
            this.header = header;
        }

        public override string Header => header;
        public override string Member => $"{LogEntryColumn.Custom.GetAttribute<MemberNameAttribute>().Member}[{index}]";
        public int Index => index;
    }
}
