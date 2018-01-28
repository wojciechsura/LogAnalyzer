using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineLogParser
{
    class LineLogParser : ILogParser
    {
        public LogEntry Parse(string line, LogEntry lastEntry)
        {
            return new LogEntry(DateTime.MinValue, null, line);
        }

        public void Dispose()
        {

        }
    }
}
