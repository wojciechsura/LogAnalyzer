using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;

namespace RegexLogParser.Configuration
{
    public class MessageGroupDefinition : BaseGroupDefinition
    {
        public override LogEntryColumn GetColumn()
        {
            return LogEntryColumn.Message;
        }
    }
}
