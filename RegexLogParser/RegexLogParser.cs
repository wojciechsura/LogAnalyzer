using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLogParser
{
    class RegexLogParser : ILogParser
    {
        private RegexLogParserConfiguration configuration;

        public RegexLogParser(RegexLogParserConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<BaseColumnInfo> GetColumnInfos()
        {
            throw new NotImplementedException();
        }

        public LogEntry Parse(string line, LogEntry lastEntry)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
