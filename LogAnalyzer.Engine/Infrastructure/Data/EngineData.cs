using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data
{
    class EngineData : ILogReaderEngineDataView
    {
        private List<LogEntry> parsedEntries;

        public List<LogEntry> ResultLogEntries => parsedEntries;
    }
}
