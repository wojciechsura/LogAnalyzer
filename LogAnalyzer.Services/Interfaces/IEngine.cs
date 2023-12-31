﻿using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public class EngineStoppedEventArgs : EventArgs
    {
        public EngineStoppedEventArgs(object stopObject)
        {
            StopObject = stopObject;
        }

        public object StopObject { get; set; }
    }

    public delegate void EngineStoppedDelegate(object sender, EngineStoppedEventArgs args);

    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(bool status)
        {
            Status = status;
        }

        public bool Status { get; }
    }

    public delegate void StatusChangedDelegate(object sender, StatusChangedEventArgs args);

    public interface IEngine
    {
        void NotifySourceReady();
        ObservableRangeCollection<LogRecord> LogRecords { get; }
        ObservableRangeCollection<LogRecord> SearchResults { get; }
        IList<LogEntry> LogEntries { get; }
        void Stop(Action stopAction);
        List<BaseColumnInfo> GetColumnInfos();

        void AddBookmark(string name, LogRecord logRecord);
        LogRecord GetLogRecordForBookmark(string name);

        void AddProfilingEntry(LogEntry entry);
        void RemoveProfilingEntry(LogEntry entry);
        bool IsProfilingEntry(LogEntry entry);
        void ClearProfilingEntries();

        void AddNote(string note, LogRecord logRecord);
        void RemoveNote(LogRecord logRecord);
        string GetNote(LogRecord selectedLogEntry);

        HighlightConfig HighlightConfig { get; set; }
        FilterConfig FilterConfig { get; set; }
        SearchConfig SearchConfig { get; set; }

        DateTime GetFirstFilteredTime();
        LogRecord FindFirstRecordAfter(DateTime resultDate);
        LogRecord QuickSearch(string phrase, LogRecord searchFrom, bool down, bool searchCaseSensitive, bool searchWholeWords, bool searchRegex);

        event StatusChangedDelegate LoadingStatusChanged;
        event StatusChangedDelegate ProcessingStatusChanged;

        void SetProcessingProfile(FilterConfig filterConfig, HighlightConfig highlightConfig);
    }
}
