using AutoMapper;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Models.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class LogReader
    {
        // Private constants --------------------------------------------------

        private const int MAX_PROCESSED_LINES = 200;

        // Private classes ----------------------------------------------------

        private class ProcessingArgument
        {
            public IReadOnlyLogEntry LastLogEntry { get; set; }
        }

        private class ProcessingResult
        {
            public bool ReplaceLast { get; set; }
            public List<LogEntry> ParsedEntries { get; set; }
        }

        // Private fields -----------------------------------------------------

        private readonly ILogSource logSource;
        private readonly ILogParser logParser;
        private readonly EventBus eventBus;
        private readonly IMapper mapper;
        private readonly ILogReaderEngineDataView data;

        private readonly BackgroundWorker backgroundWorker;
        private bool workerRunning = false;
        private bool restart = false;

        // Private methods ----------------------------------------------------

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    // TODO
                }
                else
                {
                    ProcessingResult result = e.Result as ProcessingResult;
                    if (result == null)
                        throw new InvalidOperationException("Invalid processing result!");

                    if (result.ReplaceLast)
                    {
                        if (data.ResultLogEntries.Count == 0)
                            throw new InvalidOperationException("Cannot replace last item!");

                        data.ResultLogEntries[data.ResultLogEntries.Count - 1] = result.ParsedEntries[0];
                        result.ParsedEntries.RemoveAt(0);

                        // Only last item of parsed entries may be updated, because
                        // lines following this entry may contain eg. exception details
                        // or callstack lines, which are appended to last entry's message.
                        eventBus.Send(new LastParsedEntriesItemReplacedEvent());
                    }

                    if (result.ParsedEntries.Count > 0)
                    {
                        int start = data.ResultLogEntries.Count;
                        int count = result.ParsedEntries.Count;

                        data.ResultLogEntries.AddRange(result.ParsedEntries);

                        eventBus.Send(new AddedNewParsedEntriesEvent(start, count));
                    }

                    if (result.ParsedEntries.Count == MAX_PROCESSED_LINES)
                    {
                        StartWorker();
                        return;
                    }

                    if (restart)
                    {
                        restart = false;
                        StartWorker();
                        return;
                    }
                }
            }
            finally
            {
                workerRunning = false;
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as ProcessingArgument;

            var processedItems = new List<LogEntry>();
            bool replaceFirst = false;
            LogEntry lastEntry = null;

            if (argument.LastLogEntry != null)
            {
                lastEntry = mapper.Map<LogEntry>(argument.LastLogEntry);
                processedItems.Add(lastEntry);
                replaceFirst = true;
            }

            int linesProcessed = 0;

            string line;
            do
            {
                line = logSource.GetLine();
                if (line != null)
                {
                    linesProcessed++;

                    LogEntry newEntry = logParser.Parse(line, lastEntry);
                    if (newEntry != null)
                        lastEntry = newEntry;
                }
            }
            while (line != null && linesProcessed < MAX_PROCESSED_LINES);

            if (linesProcessed == 0)
            {
                ProcessingResult result = new ProcessingResult();
                result.ParsedEntries = null;
                result.ReplaceLast = false;
                e.Result = result;
                return;
            }
            else
            {
                ProcessingResult result = new ProcessingResult();
                result.ParsedEntries = processedItems;
                result.ReplaceLast = replaceFirst;
            }
        }

        private void StartWorker()
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(new ProcessingArgument
            {
                LastLogEntry = data.ResultLogEntries.LastOrDefault()
            });
        }

        // Public methods -----------------------------------------------------

        public LogReader(ILogSource logSource, ILogParser logParser, EventBus eventBus, IMapper mapper, ILogReaderEngineDataView data)
        {
            this.logSource = logSource;
            this.logParser = logParser;
            this.eventBus = eventBus;
            this.mapper = mapper;
            this.data = data;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public void NotifySourceReady()
        {
            if (workerRunning)
            {
                restart = true;
                return;
            }
            else
            {
                StartWorker();
            }
        }
    }
}
