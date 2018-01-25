using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data.Interfaces
{
    interface ILogReaderEngineDataView
    {
        List<LogEntry> ResultLogEntries { get; }
    }
}
