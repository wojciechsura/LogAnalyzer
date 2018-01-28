using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;
using LogAnalyzer.Types;

namespace LogAnalyzer.Engine.Infrastructure.Data.Interfaces
{
    interface ILogHighlighterEngineDataView
    {
        int GetFilteredLogEntryCount();
        List<FilteredLogEntry> GetFilteredLogEntries(int start, int count);
        ObservableRangeCollection<HighlightedLogEntry> HighlightedLogEntries { get; }
    }
}
