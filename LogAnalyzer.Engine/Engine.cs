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
            public StopToken(object stopData)
            {
                this.StopData = stopData;
            }

            public bool ReaderStopped { get; set; } = false;
            public bool FilterStopped { get; set; } = false;
            public bool HighlighterStopped { get; set; } = false;
            public object StopData { get; }

            public bool AllStopped => ReaderStopped && FilterStopped && HighlighterStopped;
        }

        // Private fields -----------------------------------------------------

        private readonly EventBus eventBus;
        private readonly LogReader logReader;
        private readonly LogFilter logFilter;
        private readonly LogHighlighter logHighlighter;
        private readonly EngineData data;

        private StopToken stopToken = null;

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
            logReader.NotifySourceReady();
        }

        public void Stop(object stopData)
        {
            if (stopToken != null)
                throw new InvalidOperationException("Already stopping!");

            stopToken = new StopToken(stopData);
            logReader.Stop(() =>
                {
                    stopToken.ReaderStopped = true;
                    CheckStop();
                });
            logFilter.Stop(() =>
                {
                    stopToken.FilterStopped = true;
                    CheckStop();
                });
            logHighlighter.Stop(() =>
                {
                    stopToken.HighlighterStopped = true;
                    CheckStop();
                });
        }

        public void CheckStop()
        {
            if (stopToken.AllStopped)
            {
                EngineStopped?.Invoke(this, new EngineStoppedEventArgs(stopToken.StopData));
            }
        }

        // Public properties --------------------------------------------------

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries => data.HighlightedLogEntries;

        public event EngineStoppedDelegate EngineStopped;
    }
}
