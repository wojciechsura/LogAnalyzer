using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
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

        public List<ColumnInfo> GetColumns()
        {
            List<ColumnInfo> result = new List<ColumnInfo>();
            result.Add(new ColumnInfo(LogEntryColumn.Message));

            return result;
        }

        public void Dispose()
        {

        }
    }
}
