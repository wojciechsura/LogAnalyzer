using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;

namespace DatabaseLogParser.Configuration
{
    public class MessageFieldDefinition : BaseFieldDefinition
    {
        public override LogEntryColumn GetColumn()
        {
            return LogEntryColumn.Message;
        }
    }
}
