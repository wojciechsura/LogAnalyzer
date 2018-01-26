using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data.Interfaces
{
    interface ILogFilterEngineDataView
    {
        int GetLogEntryCount();
        List<LogEntry> GetLogEntries(int start, int count);

        List<FilteredLogEntry> FilteredLogEntries { get; }
    }
}
