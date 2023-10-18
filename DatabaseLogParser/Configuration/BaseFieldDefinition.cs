using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;

namespace DatabaseLogParser.Configuration
{
    public abstract class BaseFieldDefinition
    {
        public abstract LogEntryColumn GetColumn();

        public string Field { get; set; }
    }
}
