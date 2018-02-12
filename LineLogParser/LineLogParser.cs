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
        (BaseLogEntry, ParserOperation) ILogParser.Parse(string line, BaseLogEntry lastEntry)
        {
            return (new BaseLogEntry(DateTime.MinValue, null, line, null), ParserOperation.AddNew);
        }
        
        public List<BaseColumnInfo> GetColumnInfos()
        {
            List<BaseColumnInfo> result = new List<BaseColumnInfo>
            {
                new CommonColumnInfo(LogEntryColumn.Message)
            };

            return result;
        }

        public void Dispose()
        {

        }
    }
}
