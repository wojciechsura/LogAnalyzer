using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;

namespace RegexLogParser.Configuration
{
    public class GroupDefinition
    {
        public LogEntryColumn Column { get; set; }
        public BaseGroupDefinitionData Data { get; set; }
    }
}
