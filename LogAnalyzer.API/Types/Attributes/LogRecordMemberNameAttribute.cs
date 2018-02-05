using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Types.Attributes
{
    public class LogRecordMemberNameAttribute : Attribute
    {
        public LogRecordMemberNameAttribute(string member)
        {
            Member = member;
        }

        public string Member { get; }
    }
}
