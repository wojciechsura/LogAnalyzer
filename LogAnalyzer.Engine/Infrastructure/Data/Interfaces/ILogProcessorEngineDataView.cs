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
        List<LogRecord> BuildDataForFiltering(int start, int count);
        List<LogEntry> BuildDataForHighlighting(int start, int count);
        List<HighlightedLogRecord> BuildDataForSearching(int start, int count);
        ObservableRangeCollection<HighlightedLogRecord> HighlightedLogEntries { get; }
        ObservableRangeCollection<HighlightedLogRecord> FoundEntries { get; }
        bool IsProcessing { get; set; }
    }
}
