using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Engine.Interfaces;
using LogAnalyzer.Models.Engine;
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

        // Private fields -----------------------------------------------------

        private readonly EventBus eventBus;
        private readonly IMapper mapper;
        private readonly ILogFilterEngineDataView data;
        private readonly Queue<BaseQueueItem> queue;

        private BackgroundWorker backgroundWorker;
        private bool workerRunning = false;

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
                    FilteredLogEntry filteredLogEntry = mapper.Map<FilteredLogEntry>(entry);
                    filteredLogEntry.LogEntryIndex = index;
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

                ProcessQueue();
            }
        }

        private void StartWorker(ProcessingArgument argument)
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);
        }

        private void ProcessQueue()
        {
            if (workerRunning)
                throw new InvalidOperationException("Worker cannot be running when processing queue!");

            while (queue.Count > 0)
            {
                BaseQueueItem item = queue.Dequeue();

                if (item is ProcessItemsQueueItem processItems)
                {
                    if (processItems.Range.Start < 0 || processItems.Range.Start >= data.GetLogEntryCount() || processItems.Range.Start + processItems.Range.Count > data.GetLogEntryCount())
                        throw new ArgumentException("Invalid range of LogEntries to process!");

                    ProcessingArgument argument = new ProcessingArgument
                    {
                        InputEntries = data.GetLogEntries(processItems.Range.Start, processItems.Range.Count),
                        Range = processItems.Range
                    };

                    StartWorker(argument);
                    break;
                }
                else if (item is ClearFilteredItemsQueueItem)
                {
                    data.FilteredLogEntries.Clear();

                    eventBus.Send(new ClearedFilteredEntriesEvent());

                    if (data.GetLogEntryCount() > 0)
                    {
                        ProcessingArgument argument = new ProcessingArgument
                        {
                            InputEntries = data.GetLogEntries(0, data.GetLogEntryCount()),
                            Range = new Range(0, data.GetLogEntryCount())
                        };

                        StartWorker(argument);
                        break;
                    }
                }
                else if (item is HandleLogEntryReplacedQueueItem replacedItem)
                {
                    // Searching for filtered item built on replaced entry
                    if (data.FilteredLogEntries.Count > 0)
                    {
                        if (data.FilteredLogEntries[data.FilteredLogEntries.Count - 1].LogEntryIndex == replacedItem.Index)
                        {
                            int removedFilteredItemIndex = data.FilteredLogEntries.Count - 1;

                            data.FilteredLogEntries.RemoveAt(removedFilteredItemIndex);

                            eventBus.Send(new RemovedLastFilteredLogEntryEvent(removedFilteredItemIndex));

                            ProcessingArgument argument = new ProcessingArgument
                            {
                                InputEntries = data.GetLogEntries(replacedItem.Index, 1),
                                Range = new Range(replacedItem.Index, 1)
                            };

                            StartWorker(argument);
                            break;
                        }
                    }
                }
                else
                    throw new InvalidOperationException("Invalid queue item type!");
            }
        }

        // Public methods -----------------------------------------------------

        public LogFilter(EventBus eventBus, IMapper mapper, ILogFilterEngineDataView data)
        {
            this.eventBus = eventBus;
            this.mapper = mapper;
            this.data = data;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            queue = new Queue<BaseQueueItem>();

            eventBus.Register<AddedNewParsedEntriesEvent>(this);
            eventBus.Register<LastParsedEntriesItemReplacedEvent>(this);
        }

        public void Receive(AddedNewParsedEntriesEvent @event)
        {
            ProcessItemsQueueItem queueItem = new ProcessItemsQueueItem
            {
                Range = new Range(@event.Start, @event.Count)
            };

            queue.Enqueue(queueItem);

            if (!workerRunning)
                ProcessQueue();
        }

        public void Receive(LastParsedEntriesItemReplacedEvent @event)
        {
            HandleLogEntryReplacedQueueItem logEntryReplacedItem = new HandleLogEntryReplacedQueueItem
            {
                Index = @event.Index
            };

            queue.Enqueue(logEntryReplacedItem);

            if (!workerRunning)
                ProcessQueue();
        }
    }
}
