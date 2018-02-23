using AutoMapper;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Models.Interfaces;
using LogAnalyzer.API.Types;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class LogReader
    {
        // Private constants --------------------------------------------------

        private const int MAX_PROCESSED_LINES = 1024 * 100;

        // Private classes ----------------------------------------------------

        private class ProcessingArgument
        {
            public BaseLogEntry LastLogEntry { get; set; }
        }

        private class ProcessingResult
        {
            public bool ReplaceLast { get; set; }
            public List<BaseLogEntry> ParsedEntries { get; set; }
            public bool MoreData { get; set; }
        }

        private class StopData
        {
            public StopData(Action afterStop)
            {
                this.AfterStop = afterStop;
            }

            public Action AfterStop { get; set; }
        }

        private enum State
        {
            Working,
            Stopping,
            Stopped
        }

        // Private fields -----------------------------------------------------

        private readonly ILogSource logSource;
        private readonly ILogParser logParser;
        private readonly EventBus eventBus;
        private readonly ILogReaderEngineDataView data;
        private readonly ILogEntryMetaHandler handler;
        private readonly BackgroundWorker backgroundWorker;
        private bool workerRunning = false;
        private bool restart = false;
        private State state = State.Working;
        private StopData stopData = null;

        // Private methods ----------------------------------------------------

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            var argument = e.Argument as ProcessingArgument;

            var processedItems = new List<BaseLogEntry>();
            bool replaceFirst = false;
            BaseLogEntry lastEntry = null;

            if (argument.LastLogEntry != null)
            {
                processedItems.Add(argument.LastLogEntry);
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

                    (BaseLogEntry entry, ParserOperation operation) = logParser.Parse(line, lastEntry);

                    if (operation == ParserOperation.ReplaceLast)
                    {
                        processedItems[processedItems.Count - 1] = entry ?? throw new InvalidOperationException("ReplaceLast operation must yield entry!");
                        lastEntry = entry;
                    }
                    else if (operation == ParserOperation.AddNew)
                    {
                        if (entry == null)
                            throw new InvalidOperationException("AddNew operation must yield entry!");

                        processedItems.Add(entry);
                        lastEntry = entry;
                    }
                    else if (operation == ParserOperation.None)
                    {
                        if (entry != null)
                            throw new InvalidOperationException("None operation must not yield entry!");
                    }
                }
            }
            while (line != null && linesProcessed < MAX_PROCESSED_LINES);

            if (linesProcessed == 0)
            {
                ProcessingResult result = new ProcessingResult
                {
                    ParsedEntries = null,
                    ReplaceLast = false,
                    MoreData = false                    
                };
                e.Result = result;
                return;
            }
            else
            {
                ProcessingResult result = new ProcessingResult
                {
                    ParsedEntries = processedItems,
                    ReplaceLast = replaceFirst,
                    MoreData = linesProcessed == MAX_PROCESSED_LINES
                };
                e.Result = result;
                return;
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(false));

            if (state == State.Stopping)
            {
                DoStopReader();
            }
            else if (state == State.Stopped)
            {
                throw new InvalidOperationException("Already stopped, cannot process data!");
            }
            else if (state == State.Working)
            {
                bool runAgain = false;

                try
                {
                    if (!e.Cancelled)
                    {
                        ProcessingResult result = e.Result as ProcessingResult;
                        if (result == null)
                            throw new InvalidOperationException("Invalid processing result!");

                        if (result.ReplaceLast)
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine($"[R]->[    ] Replacing last item with meta index {data.ResultLogEntries.LastOrDefault()?.Index}");
#endif

                            if (data.ResultLogEntries.Count == 0)
                                throw new InvalidOperationException("Cannot replace last item!");

                            data.ResultLogEntries[data.ResultLogEntries.Count - 1] = new LogEntry(result.ParsedEntries[0], data.ResultLogEntries[data.ResultLogEntries.Count - 1].Index, handler);
                            result.ParsedEntries.RemoveAt(0);

                            // Only last item of parsed entries may be updated, because
                            // lines following this entry may contain eg. exception details
                            // or callstack lines, which are appended to last entry's message.
                            eventBus.Send(new LastParsedEntriesItemReplacedEvent(data.ResultLogEntries[data.ResultLogEntries.Count - 1].Index));
                        }

                        if (result.ParsedEntries.Count > 0)
                        {
                            int start = data.ResultLogEntries.Count;
                            int count = result.ParsedEntries.Count;

#if DEBUG
                            System.Diagnostics.Debug.WriteLine($"[R]->[    ] Added new parsed entries - start: {start}, count: {count}");
#endif
                            int index = data.ResultLogEntries.Count + 1;
                            for (int i = 0; i < result.ParsedEntries.Count; i++)
                                data.ResultLogEntries.Add(new LogEntry(result.ParsedEntries[i], index++, handler));

                            eventBus.Send(new AddedNewParsedEntriesEvent(start, count));
                        }

                        if (result.MoreData)
                        {
                            runAgain = true;
                            return;
                        }

                        if (restart)
                        {
                            restart = false;
                            runAgain = true;
                            return;
                        }
                    }
                }
                finally
                {
                    workerRunning = false;

                    if (runAgain)
                        StartWorker();
                    else
                        LoadingFinished();
                }
            }
            else
                throw new InvalidOperationException("Invalid state!");
        }

        private void LoadingFinished()
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(false));
        }

        internal List<BaseColumnInfo> GetColumnInfos()
        {
            return logParser.GetColumnInfos();
        }

        private void StartWorker()
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot start worker when stopped!");

            StatusChanged?.Invoke(this, new StatusChangedEventArgs(true));

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(new ProcessingArgument
            {
                LastLogEntry = data.ResultLogEntries.LastOrDefault()
            });
        }

        private void DoStopReader()
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(false));

            logSource.Dispose();
            logParser.Dispose();

            state = State.Stopped;

            stopData.AfterStop();
        }

        // Public methods -----------------------------------------------------

        public LogReader(ILogSource logSource, ILogParser logParser, EventBus eventBus, ILogReaderEngineDataView data, ILogEntryMetaHandler handler)
        {
            this.logSource = logSource;
            this.logParser = logParser;
            this.eventBus = eventBus;
            this.data = data;
            this.handler = handler;

            backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public void Stop(Action afterStop)
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Already stopping or stopped!");

            state = State.Stopping;
            stopData = new StopData(afterStop);

            if (workerRunning)
            {
                backgroundWorker.CancelAsync();
                // RunWorkerCompleted will take care of stopping the reader.
            }
            else
            {
                DoStopReader();
            }
        }

        public void NotifySourceReady()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot perform this operation, reader is stopped!");

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

        public StatusChangedDelegate StatusChanged;
    }
}
