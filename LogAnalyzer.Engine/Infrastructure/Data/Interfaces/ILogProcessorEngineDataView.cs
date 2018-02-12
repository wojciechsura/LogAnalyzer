using LogAnalyzer.API.Models;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data.Interfaces
{
    interface ILogProcessorEngineDataView
    {
        int GetLogEntryCount();
        List<LogEntry> BuildDataForFiltering(int start, int count);
        List<LogEntry> BuildDataForHighlighting(int start, int count);
        List<LogRecord> BuildDataForSearching(int start, int count);
        ObservableRangeCollection<LogRecord> HighlightedLogEntries { get; }
        ObservableRangeCollection<LogRecord> FoundEntries { get; }
        bool IsProcessing { get; set; }
    }
}
