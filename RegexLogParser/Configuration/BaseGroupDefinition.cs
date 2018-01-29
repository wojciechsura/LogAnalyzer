using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;

namespace RegexLogParser.Configuration
{
    public abstract class BaseGroupDefinition
    {
        public abstract LogEntryColumn GetColumn();
    }
}
