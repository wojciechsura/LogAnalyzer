using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System.Collections.Concurrent;
using System.ComponentModel;
using LogAnalyzer.Engine.Components;
using LogAnalyzer.Engine.Infrastructure.Data;
using AutoMapper;
using LogAnalyzer.Types;
using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Engine.Infrastructure.Processing;
using LogAnalyzer.Models.Types;
using LogAnalyzer.API.Models.Interfaces;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Engine
{
    class Engine : IEngine, ILogEntryMetaHandler
    {
        // Private types ------------------------------------------------------

        private class StopToken
        {
            public StopToken(Action stopAction)
            {
                this.StopAction = stopAction;
            }

            public bool ReaderStopped { get; set; } = false;
            public bool ProcessorStopped { get; set; } = false;
            public Action StopAction { get; }

            public bool AllStopped => ReaderStopped && ProcessorStopped;
        }

        private enum State
        {
            Working,
            Stopping,
            Stopped
        }

        // Private fields -----------------------------------------------------

        private readonly EventBus eventBus;
        private readonly LogReader logReader;
        private readonly LogProcessor logProcessor;
        private readonly EngineData data;

        private readonly List<BaseColumnInfo> columnInfos;

        private HighlightConfig highlightConfig;
        private FilterConfig filterConfig;
        private SearchConfig searchConfig;

        private readonly List<BookmarkEntry> bookmarks;
        private readonly SortedList<int, ProfilingEntry> profilingEntries;
        private readonly SortedList<int, string> notes;

        private State state = State.Working;
        private StopToken stopToken = null;

        // Private methods ----------------------------------------------------

        private void CheckStopCallback()
        {
            if (stopToken.AllStopped)
            {
                state = State.Stopped;
                stopToken.StopAction?.Invoke();
            }
        }

        private void SetHighlightConfig(HighlightConfig value)
        {
            highlightConfig = value;

            LogHighlighterConfig config = new LogHighlighterConfig(value, GetColumnInfos());
            logProcessor.SetHighlighterConfig(config);
        }

        private void SetFilterConfig(FilterConfig value)
        {
            filterConfig = value;

            LogFilteringConfig config = new LogFilteringConfig(value, GetColumnInfos());
            logProcessor.SetFilteringConfig(config);
        }

        private void SetSearchConfig(SearchConfig value)
        {
            searchConfig = value;

            LogSearchConfig config = new LogSearchConfig(value, GetColumnInfos());
            logProcessor.SetSearchConfig(config);
        }

        private void RemoveBookmark(string name)
        {
            var entry = bookmarks.SingleOrDefault(b => b.Name == name);
            if (entry != null)
            {
                bookmarks.Remove(entry);
                entry.LogEntry.NotifyBookmarksChanged();
            }
        }

        private void RecalcProfiling(int startIndex)
        {
            if (profilingEntries.Count == 0)
                return;

            ProfilingEntry first = profilingEntries.First().Value;

            while (startIndex < profilingEntries.Count)
            {
                ProfilingEntry current = profilingEntries.Values[startIndex];

                if (startIndex == 0)
                {
                    current.FromPrevious = TimeSpan.Zero;
                    current.FromStart = TimeSpan.Zero;
                }
                else
                {
                    ProfilingEntry previous = profilingEntries.Values[startIndex - 1];

                    current.FromPrevious = current.Entry.Date - previous.Entry.Date;
                    current.FromStart = current.Entry.Date - first.Entry.Date;
                }

                current.Step = startIndex + 1;
                current.Entry.NotifyProfilingChanged();

                startIndex++;
            }
        }

        private static Func<string, bool> BuildQuickSearchPredicate(string phrase, bool searchCaseSensitive, bool searchWholeWords, bool searchRegex)
        {
            Func<string, bool> predicate;
            if (!searchCaseSensitive && !searchWholeWords && !searchRegex)
            {
                string lowerPhrase = phrase.ToLower();
                predicate = (content) => content.ToLower().Contains(lowerPhrase);
            }
            else if (searchCaseSensitive && !searchWholeWords && !searchRegex)
            {
                predicate = (content) => content.Contains(phrase);
            }
            else if (!searchCaseSensitive && searchWholeWords && !searchRegex)
            {
                string regexPhrase = $"\\b{Regex.Escape(phrase)}\\b";
                Regex regex = new Regex(regexPhrase, RegexOptions.IgnoreCase);

                predicate = (content) => regex.IsMatch(content);
            }
            else if (searchCaseSensitive && searchWholeWords && !searchRegex)
            {
                string regexPhrase = $"\\b{Regex.Escape(phrase)}\\b";
                Regex regex = new Regex(regexPhrase);

                predicate = (content) => regex.IsMatch(content);
            }
            else if (!searchCaseSensitive && !searchWholeWords && searchRegex)
            {
                Regex regex = new Regex(phrase, RegexOptions.IgnoreCase);
                predicate = (content) => regex.IsMatch(content);
            }
            else if (searchCaseSensitive && !searchWholeWords && searchRegex)
            {
                Regex regex = new Regex(phrase);
                predicate = (content) => regex.IsMatch(content);
            }
            else if (!searchCaseSensitive && searchWholeWords && searchRegex)
            {
                Regex regex = new Regex($"\\b{phrase}\\b", RegexOptions.IgnoreCase);
                predicate = (content) => regex.IsMatch(content);
            }
            else
            {
                Regex regex = new Regex($"\\b{phrase}\\b");
                predicate = (content) => regex.IsMatch(content);
            }

            return predicate;
        }

        // ILogEntryMetaHandler implementation --------------------------------

        IEnumerable<string> ILogEntryMetaHandler.GetBookmarks(LogEntry logEntry)
        {
            return bookmarks
                .Where(b => b.LogEntry == logEntry)
                .OrderBy(b => b.Name)
                .Select(b => b.Name)
                .ToList();
        }

        bool ILogEntryMetaHandler.IsProfilingPoint(LogEntry logEntry)
        {
            return profilingEntries.ContainsKey(logEntry.Index);
        }

        TimeSpan ILogEntryMetaHandler.TimeSpanFromStart(LogEntry logEntry)
        {
            return profilingEntries.ContainsKey(logEntry.Index) ? profilingEntries[logEntry.Index].FromStart : TimeSpan.Zero;
        }

        TimeSpan ILogEntryMetaHandler.TimeSpanFromPrevious(LogEntry logEntry)
        {
            return profilingEntries.ContainsKey(logEntry.Index) ? profilingEntries[logEntry.Index].FromPrevious : TimeSpan.Zero;
        }

        int ILogEntryMetaHandler.GetProfilingStep(LogEntry logEntry)
        {
            return profilingEntries.ContainsKey(logEntry.Index) ? profilingEntries[logEntry.Index].Step : -1;
        }

        bool ILogEntryMetaHandler.HasNote(LogEntry logEntry)
        {
            return notes.ContainsKey(logEntry.Index);
        }

        string ILogEntryMetaHandler.Note(LogEntry logEntry)
        {
            if (notes.ContainsKey(logEntry.Index))
                return notes[logEntry.Index];

            return null;
        }

        // Public methods -----------------------------------------------------

        public Engine(ILogSource logSource, ILogParser logParser)
        {
            eventBus = new EventBus();
            data = new EngineData();

            logReader = new LogReader(logSource, logParser, eventBus, data, this);
            columnInfos = logReader.GetColumnInfos();

            logProcessor = new LogProcessor(eventBus, data);

            highlightConfig = new HighlightConfig
            {
                HighlightEntries = new List<HighlightEntry>()
            };

            filterConfig = new FilterConfig
            {
                FilterEntries = new List<FilterEntry>(),
                DefaultAction = FilterAction.Include
            };

            bookmarks = new List<BookmarkEntry>();
            profilingEntries = new SortedList<int, ProfilingEntry>();
            notes = new SortedList<int, string>();
        }

        public void NotifySourceReady()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot perform work when stopping or stopped!");

            logReader.NotifySourceReady();
        }

        public void Stop(Action stopAction)
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Already stopping!");

            state = State.Stopping;
            stopToken = new StopToken(stopAction);

            logReader.Stop(() =>
                {
                    stopToken.ReaderStopped = true;
                    CheckStopCallback();
                });
            logProcessor.Stop(() =>
                {
                    stopToken.ProcessorStopped = true;
                    CheckStopCallback();
                });
        }

        public List<BaseColumnInfo> GetColumnInfos()
        {
            return columnInfos;
        }

        public DateTime GetFirstFilteredTime()
        {
            if (data.HighlightedLogEntries.Count > 0)
                return data.HighlightedLogEntries.First().LogEntry.Date;
            else
                return DateTime.Now;
        }

        public LogRecord FindFirstRecordAfter(DateTime resultDate)
        {
            return data.HighlightedLogEntries.FirstOrDefault(e => e.LogEntry.Date.CompareTo(resultDate) >= 0);            
        }

        public void AddBookmark(string name, LogRecord logRecord)
        {
            if (!data.ResultLogEntries.Any(le => le == logRecord.LogEntry))
                throw new ArgumentException("Invalid log entry!");

            // Removing existing
            RemoveBookmark(name);

            bookmarks.Add(new BookmarkEntry(name, logRecord.LogEntry));
            logRecord.LogEntry.NotifyBookmarksChanged();
        }

        public LogRecord GetLogRecordForBookmark(string name)
        {
            var entry = bookmarks
                .SingleOrDefault(b => b.Name == name)
                ?.LogEntry;

            if (entry == null)
                return null;

            return data.HighlightedLogEntries
                .Where(r => r.LogEntry == entry)
                .SingleOrDefault();
        }

        public void AddProfilingEntry(LogEntry entry)
        {
            if (profilingEntries.ContainsKey(entry.Index))
            {
                return;
            }
            else
            {
                ProfilingEntry profilingEntry = new ProfilingEntry(entry);
                profilingEntries.Add(entry.Index, profilingEntry);
                int newEntryIndex = profilingEntries.IndexOfKey(entry.Index);
                RecalcProfiling(newEntryIndex);
            }
        }

        public void RemoveProfilingEntry(LogEntry entry)
        {
            int index = profilingEntries.IndexOfKey(entry.Index);
            profilingEntries.RemoveAt(index);

            entry.NotifyProfilingChanged();
            RecalcProfiling(index);
        }

        public bool IsProfilingEntry(LogEntry entry)
        {
            return profilingEntries.ContainsKey(entry.Index);
        }

        public void ClearProfilingEntries()
        {
            List<LogEntry> entries = profilingEntries
                .Select(kv => kv.Value.Entry)
                .ToList();

            profilingEntries.Clear();

            foreach (var entry in entries)
                entry.NotifyProfilingChanged();
        }

        public void AddNote(string note, LogRecord logRecord)
        {
            if (!data.ResultLogEntries.Any(le => le == logRecord.LogEntry))
                throw new ArgumentException("Invalid log entry!");

            if (string.IsNullOrEmpty(note.Trim()))
            {
                RemoveNote(logRecord);
            }
            else
            {
                notes[logRecord.LogEntry.Index] = note;
                logRecord.LogEntry.NotifyNoteChanged();
            }
        }

        public string GetNote(LogRecord logRecord)
        {
            if (notes.ContainsKey(logRecord.LogEntry.Index))
                return notes[logRecord.LogEntry.Index];
            else
                return null;
        }

        public void RemoveNote(LogRecord logRecord)
        {
            if (!data.ResultLogEntries.Any(le => le == logRecord.LogEntry))
                throw new ArgumentException("Invalid log entry!");

            if (notes.ContainsKey(logRecord.LogEntry.Index))
                notes.Remove(logRecord.LogEntry.Index);

            logRecord.LogEntry.NotifyNoteChanged();
        }

        public LogRecord QuickSearch(string phrase, LogRecord searchFrom, bool down, bool searchCaseSensitive, bool searchWholeWords, bool searchRegex)
        {
            int index = data.HighlightedLogEntries.IndexOf(searchFrom);
            if (index == -1)
            {
                index = down ? -1 : data.HighlightedLogEntries.Count;
            }

            Func<string, bool> predicate = BuildQuickSearchPredicate(phrase, searchCaseSensitive, searchWholeWords, searchRegex);

            Func<LogRecord, bool> logRecordPredicate = entry => (logReader.GetColumnInfos()
                .Any(c => predicate(c.GetStringValue(entry.LogEntry))));

            do
            {
                if (down)
                    index++;
                else
                    index--;
            }
            while (index >= 0 && index < data.HighlightedLogEntries.Count && !logRecordPredicate(data.HighlightedLogEntries[index]));

            if (index >= 0 && index < data.HighlightedLogEntries.Count)
                return data.HighlightedLogEntries[index];
            else
                return null;
        }

        public void SetProcessingProfile(FilterConfig filterConfig, HighlightConfig highlightConfig)
        {
            SetFilterConfig(filterConfig);
            SetHighlightConfig(highlightConfig);
        }

        // Public properties --------------------------------------------------

        public ObservableRangeCollection<LogRecord> LogRecords => data.HighlightedLogEntries;

        public ObservableRangeCollection<LogRecord> SearchResults => data.FoundEntries;

        public HighlightConfig HighlightConfig
        {
            get
            {
                return highlightConfig;
            }
            set
            {
                SetHighlightConfig(value);
            }
        }

        public FilterConfig FilterConfig
        {
            get
            {
                return filterConfig;
            }
            set
            {
                SetFilterConfig(value);
            }
        }

        public SearchConfig SearchConfig
        {
            get
            {
                return searchConfig;
            }
            set
            {
                SetSearchConfig(value);
            }
        }

        public IReadOnlyList<LogEntry> LogEntries => data.ResultLogEntries;

        public event StatusChangedDelegate LoadingStatusChanged
        {
            add
            {
                logReader.StatusChanged += value;
            }
            remove
            {
                logReader.StatusChanged -= value;
            }
        }

        public event StatusChangedDelegate ProcessingStatusChanged
        {
            add
            {
                logProcessor.StatusChanged += value;
            }
            remove
            {
                logReader.StatusChanged -= value;
            }
        }
    }
}
