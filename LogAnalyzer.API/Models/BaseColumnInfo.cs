using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public abstract class BaseColumnInfo
    {
        public abstract string Header { get; }
        public abstract string Member { get; }

        public abstract string GetStringValue(LogEntry logEntry);
    }
}
