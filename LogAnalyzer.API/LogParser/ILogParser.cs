using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogParser
{
    public interface ILogParser : IDisposable
    {
        (LogEntry, ParserOperation) Parse(string line, LogEntry lastEntry);
        List<BaseColumnInfo> GetColumnInfos();
    }
}
