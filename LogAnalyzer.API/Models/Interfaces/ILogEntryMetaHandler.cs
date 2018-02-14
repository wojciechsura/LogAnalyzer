using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models.Interfaces
{
    public interface ILogEntryMetaHandler
    {
        IEnumerable<string> GetBookmarks(LogEntry logEntry);

        bool IsProfilingPoint(LogEntry logEntry);
        TimeSpan TimeSpanFromStart(LogEntry logEntry);
        TimeSpan TimeSpanFromPrevious(LogEntry logEntry);
        int GetProfilingStep(LogEntry logEntry);
    }
}
