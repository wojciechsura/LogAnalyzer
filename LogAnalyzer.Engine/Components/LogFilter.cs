using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class LogFilter : IEventListener<AddedNewParsedEntriesEvent>, IEventListener<LastParsedEntriesItemReplacedEvent>
    {
        // Private constants --------------------------------------------------

        private const int MAX_PROCESSED_ITEMS = 200;

        // Private classes ----------------------------------------------------

        private class Range
        {
            public Range(int start, int count)
            {
                Start = start;
                Count = count;
            }

            public int Start { get; set; }
            public int Count { get; set; }
        }

        private class BaseQueueItem
        {

        }

        private class ProcessItemsQueueItem : BaseQueueItem
        {
            public Range Range { get; set; }
        }

        private class ClearFilteredItemsQueueItem : BaseQueueItem
        {

        }

        private class HandleLogEntryReplacedQueueItem : BaseQueueItem
        {
            public int Index { get; set; }
        }

        private class ProcessingArgument
        {
            public Range Range { get; set; }
            public List<LogEntry> InputEntries { get; set; }
        }

        private class ProcessingResult
        {
            public Range ProcessedInputRange { get; set; }
            public List<FilteredLogEntry> Entries { get; set; }
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

        private readonly EventBus eventBus;
        private readonly ILogFilterEngineDataView data;
        private readonly Queue<BaseQueueItem> queue;
        private Range processedRange = null;

        private BackgroundWorker backgroundWorker;
        private bool workerRunning = false;
        private State state = State.Working;
        private StopData stopData = null;

        // Private methods ----------------------------------------------------

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as ProcessingArgument;
            var processedItems = new List<FilteredLogEntry>();

            int index = argument.Range.Start;
            for (int i = 0; i < argument.InputEntries.Count; i++)
            {
                var entry = argument.InputEntries[i];
                
                // TODO verify filtering condition
                if (true)
                {
                    FilteredLogEntry filteredLogEntry = new FilteredLogEntry(index, 
                        entry.Date, 
                        entry.Severity, 
                        entry.Message,
                        entry.CustomFields);
                    processedItems.Add(filteredLogEntry);
                }

                index++;
            }

            ProcessingResult result = new ProcessingResult
            {
                ProcessedInputRange = argument.Range,
                Entries = processedItems
            };
            e.Result = result;
        }

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

                    if (result.Entries.Count > 0)
                    {
                        int start = data.FilteredLogEntries.Count;
                        int count = result.Entries.Count;

                        data.FilteredLogEntries.AddRange(result.Entries);

                        eventBus.Send(new AddedNewFilteredEntriesEvent(start, count));
                    }
                }
            }
            finally
            {
                workerRunning = false;

                ContinueWork();
            }
        }

        private void StartWorker(Range range)
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot start worker when stopped!");

            var argument = new ProcessingArgument
            {
                InputEntries = data.GetLogEntries(range.Start, range.Count),
                Range = range
            };

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);
        }

        private void DoStopFilter()
        {
            queue.Clear();
            state = State.Stopped;

            stopData.AfterStop();
        }

        private bool ProcessQueue()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot process queue when stopped or stopping!");

            while (queue.Count > 0)
            {
                BaseQueueItem item = queue.Dequeue();

                if (item is ProcessItemsQueueItem processItems)
                {
                    if (processItems.Range.Start < 0 || processItems.Range.Start >= data.GetLogEntryCount() || processItems.Range.Start + processItems.Range.Count > data.GetLogEntryCount())
                        throw new ArgumentException("Invalid range of LogEntries to process!");

                    StartWorker(processItems.Range);
                    return true;
                }
                else if (item is ClearFilteredItemsQueueItem)
                {
                    data.FilteredLogEntries.Clear();

                    eventBus.Send(new ClearedFilteredEntriesEvent());
                    continue;
                }
                else if (item is HandleLogEntryReplacedQueueItem replacedItem)
                {
                    // Searching for filtered item built on replaced entry
                    if (data.FilteredLogEntries.Count > 0)
                    {
                        if (data.FilteredLogEntries[data.FilteredLogEntries.Count - 1].LogEntryIndex > replacedItem.Index)
                            throw new InvalidOperationException("Replaced last LogEntry is not last item in FilteredLogItems!");

                        if (data.FilteredLogEntries[data.FilteredLogEntries.Count - 1].LogEntryIndex == replacedItem.Index)
                        {
                            // Remove item and notify
                            int removedFilteredItemIndex = data.FilteredLogEntries.Count - 1;
                            data.FilteredLogEntries.RemoveAt(removedFilteredItemIndex);

                            eventBus.Send(new RemovedLastFilteredLogEntryEvent(removedFilteredItemIndex, replacedItem.Index));

                            // Re-process the item immediately not to lose data integrity
                            Range range = new Range(replacedItem.Index, 1);
                            StartWorker(range);

                            return true;                            
                        }
                    }
                }
                else
                    throw new InvalidOperationException("Invalid queue item type!");
            }

            return false;
        }

        private void ProcessNextChunkOfData()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot process data when stopping or stopped!");

            if (processedRange != null)
            {
                // Items were already processed, trying to get next chunk of data
                if (data.GetLogEntryCount() > processedRange.Start + processedRange.Count)
                {
                    int start = processedRange.Start + processedRange.Count;
                    processedRange = new Range(start, Math.Min(MAX_PROCESSED_ITEMS, data.GetLogEntryCount() - start));

                    StartWorker(processedRange);
                    return;
                }
            }
            else
            {
                // Starting processing data from beginning
                if (data.GetLogEntryCount() > 0)
                {
                    processedRange = new Range(0, Math.Min(MAX_PROCESSED_ITEMS, data.GetLogEntryCount()));

                    StartWorker(processedRange);
                    return;
                }
            }
        }

        private void ContinueWork()
        {
            if (state == State.Stopping)
            {
                DoStopFilter();
            }
            else if (state == State.Stopped)
            {
                throw new InvalidOperationException("Cannot continue work when stopped!");
            }
            else if (state == State.Working)
            {
                if (workerRunning)
                    throw new InvalidOperationException("Worker cannot be running when continuing work!");

                if (!ProcessQueue())
                {
                    // No task on queue, checking if there are more entries to process
                    ProcessNextChunkOfData();
                }
            }
            else
                throw new InvalidOperationException("Invalid state!");
        }

        // Public methods -----------------------------------------------------

        public LogFilter(EventBus eventBus, ILogFilterEngineDataView data)
        {
            this.eventBus = eventBus;
            this.data = data;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            queue = new Queue<BaseQueueItem>();

            eventBus.Register<AddedNewParsedEntriesEvent>(this);
            eventBus.Register<LastParsedEntriesItemReplacedEvent>(this);
        }

        public void Stop(Action afterStop)
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Already stopping or stopped");

            eventBus.Unregister<AddedNewParsedEntriesEvent>(this);
            eventBus.Unregister<LastParsedEntriesItemReplacedEvent>(this);

            state = State.Stopping;
            stopData = new StopData(afterStop);

            if (workerRunning)
            {
                backgroundWorker.CancelAsync();
                // RunWorkerCompleted will take care of stopping the filter.
            }
            else
            {
                DoStopFilter();
            }
        }

        public void Receive(AddedNewParsedEntriesEvent @event)
        {
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot receive events when stopped!");

            // Worker will pick new chunk of data after
            // finishing current one, no need for any action
            if (workerRunning)
            {
                return;
            }
            else
            {
                // Else pick items to process automatically
                ContinueWork();
            }
        }

        public void Receive(LastParsedEntriesItemReplacedEvent @event)
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot receive events when stopped!");

            if (workerRunning)
            {
                if (@event.Index >= processedRange.Start + processedRange.Count)
                {
                    // Replaced entry will be parsed later, no need for any action
                    return;
                }
                else
                {
                    // Replaced entry is being processed right now, need to be re-processed
                    HandleLogEntryReplacedQueueItem logEntryReplacedItem = new HandleLogEntryReplacedQueueItem
                    {
                        Index = @event.Index
                    };

                    queue.Enqueue(logEntryReplacedItem);
                }
            }
            else
            {
                // Worker is not running, re-processing entry
                HandleLogEntryReplacedQueueItem logEntryReplacedItem = new HandleLogEntryReplacedQueueItem
                {
                    Index = @event.Index
                };

                queue.Enqueue(logEntryReplacedItem);

                ContinueWork();
            }
        }
    }
}
