using LogAnalyzer.Models.Engine;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IEngine
    {
        void NotifySourceReady();
        ObservableRangeCollection<HighlightedLogEntry> LogEntries { get; }
    }
}
