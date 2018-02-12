﻿using LogAnalyzer.Services.Interfaces;
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

        private HighlightConfig highlightConfig;
        private FilterConfig filterConfig;
        private SearchConfig searchConfig;

        private readonly Dictionary<string, LogRecord> bookmarks;

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

        // Public methods -----------------------------------------------------

        public Engine(ILogSource logSource, ILogParser logParser)
        {
            eventBus = new EventBus();
            data = new EngineData();
            logReader = new LogReader(logSource, logParser, eventBus, data, this);
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

            bookmarks = new Dictionary<string, LogRecord>();
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
            return logReader.GetColumnInfos();
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

        public void AddBookmark(string name, LogRecord newRecord)
        {
            ClearBookmark(newRecord);
            ClearBookmark(name);

            bookmarks[name] = newRecord;
            newRecord.LogEntry.NotifyBookmarkUpdated();
        }

        public void ClearBookmark(LogRecord record)
        {
            foreach (var item in bookmarks.Where(kvp => kvp.Value == record).ToList())
            {
                bookmarks.Remove(item.Key);

                record.LogEntry.NotifyBookmarkUpdated();
            }
        }

        public void ClearBookmark(string name)
        {
            if (bookmarks.ContainsKey(name))
            {
                var record = bookmarks[name];
                bookmarks.Remove(name);

                record.NotifyBookmarkUpdated();
            }
        }

        public LogRecord GetBookmarkedItem(string name)
        {
            if (bookmarks.ContainsKey(name))
                return bookmarks[name];
            else
                return null;
        }

        public string GetBookmarkFor(LogRecord record)
        {
            var items = bookmarks.Where(kvp => kvp.Value == record).ToList();

            if (items.Count == 1)
                return items[1].Key;
            else if (items.Count == 0)
                return null;

            throw new InvalidOperationException("More than one bookmark for record!");
        }

        // Public properties --------------------------------------------------

        public ObservableRangeCollection<LogRecord> LogEntries => data.HighlightedLogEntries;

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
    }
}
