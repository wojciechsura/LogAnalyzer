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
        (BaseLogEntry, ParserOperation) Parse(string line, BaseLogEntry lastEntry);
        List<BaseColumnInfo> GetColumnInfos();
    }
}
