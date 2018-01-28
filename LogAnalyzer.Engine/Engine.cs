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
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Engine.Components;
using LogAnalyzer.Engine.Infrastructure.Data;
using AutoMapper;
using LogAnalyzer.Types;

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

        // Public methods -----------------------------------------------------

        public Engine(ILogSource logSource, ILogParser logParser)
        {
            eventBus = new EventBus();
            data = new EngineData();
            logReader = new LogReader(logSource, logParser, eventBus, data);
            logFilter = new LogFilter(eventBus, data);
            logHighlighter = new LogHighlighter(eventBus, data);
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

        // Public properties --------------------------------------------------

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries => data.HighlightedLogEntries;
    }
}
