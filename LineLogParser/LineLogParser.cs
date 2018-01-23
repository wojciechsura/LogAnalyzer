using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.Engine;

namespace LineLogParser
{
    class LineLogParser : ILogParser
    {
        public LogEntry Parse(string line, LogEntry lastEntry)
        {
            return new LogEntry()
            {
                Message = line
            };
        }
    }
}
