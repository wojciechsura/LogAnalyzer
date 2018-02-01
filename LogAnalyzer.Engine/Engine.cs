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
using LogAnalyzer.Engine.Infrastructure.Highlighting;

namespace LogAnalyzer.Engine
{
    class Engine : IEngine
    {
        // Private types ------------------------------------------------------

        private class StopToken
        {
            public StopToken(Action stopAction)
            {
                this.StopAction = stopAction;
            }

            public bool ReaderStopped { get; set; } = false;
            public bool FilterStopped { get; set; } = false;
            public bool HighlighterStopped { get; set; } = false;
            public Action StopAction { get; }

            public bool AllStopped => ReaderStopped && FilterStopped && HighlighterStopped;
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
        private readonly LogFilter logFilter;
        private readonly LogHighlighter logHighlighter;
        private readonly EngineData data;
        private HighlightConfig highlightConfig;

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
            logHighlighter.SetConfig(config);
        }

        // Public methods -----------------------------------------------------

        public Engine(ILogSource logSource, ILogParser logParser)
        {
            eventBus = new EventBus();
            data = new EngineData();
            logReader = new LogReader(logSource, logParser, eventBus, data);
            logFilter = new LogFilter(eventBus, data);
            logHighlighter = new LogHighlighter(eventBus, data);

            highlightConfig = new HighlightConfig
            {
                HighlightEntries = new List<HighlightEntry>()
            };
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
            logFilter.Stop(() =>
                {
                    stopToken.FilterStopped = true;
                    CheckStopCallback();
                });
            logHighlighter.Stop(() =>
                {
                    stopToken.HighlighterStopped = true;
                    CheckStopCallback();
                });
        }

        public List<BaseColumnInfo> GetColumnInfos()
        {
            return logReader.GetColumnInfos();
        }

        // Public properties --------------------------------------------------

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries => data.HighlightedLogEntries;

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
    }
}
