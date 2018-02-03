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
        List<FilteredLogEntry> BuildDataForHighlighting(int start, int count);
        List<HighlightedLogEntry> BuildDataForSearching(int start, int count);
        ObservableRangeCollection<HighlightedLogEntry> HighlightedLogEntries { get; }
        ObservableRangeCollection<HighlightedLogEntry> FoundEntries { get; }
        bool IsProcessing { get; set; }
    }
}
