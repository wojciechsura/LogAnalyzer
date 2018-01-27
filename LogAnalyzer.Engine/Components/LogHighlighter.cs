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
using System.Windows.Media;

namespace LogAnalyzer.Engine.Components
{
    class LogHighlighter : IEventListener<AddedNewFilteredEntriesEvent>, IEventListener<RemovedLastFilteredLogEntryEvent>
    {
        // Private constants --------------------------------------------------

        private const int MAX_PROCESSED_ITEMS = 200;

        // Private classes ----------------------------------------------------

        private class Range
        {
            public Range (int start, int count)
            {
                Start = start;
                Count = count;
            }

            public int Start { get; }
            public int Count { get; }
        }

        private class BaseQueueItem
        {

        }

        private class ProcessItemsQueueItem : BaseQueueItem
        {
            public Range Range { get; set; }
        }

        private class ClearHighlightQueueItem : BaseQueueItem
        {

        }

        private class HandleFilteredLogEntryReplacedQueueItem : BaseQueueItem
        {
            public int LogEntryIndex { get; set; }
            public int FilteredLogEntryIndex { get; set; }
        }

        private class ProcessingArgument
        {
            public Range Range { get; set; }
            public List<FilteredLogEntry> InputEntries { get; set; }
        }

        private class ProcessingResult
        {
            public Range ProcessedInputRange { get; set; }
            public List<HighlightInfo> Entries { get; set; }
        }

        // Private fields -----------------------------------------------------

        private readonly EventBus eventBus;
        private readonly ILogHighlighterEngineDataView data;
        private readonly Queue<BaseQueueItem> queue = null;
        private Range processedRange = null;

        private BackgroundWorker backgroundWorker;
        private bool workerRunning = false;

        // Private methods ----------------------------------------------------

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as ProcessingArgument;
            var processedItems = new List<HighlightInfo>();

            for (int i = 0; i < argument.InputEntries.Count; i++)
            {
                var entry = argument.InputEntries[i];

                // TODO verify highlighting condition
                HighlightInfo info = new HighlightInfo(i % 2 == 0 ? Colors.Red : Colors.Black, Colors.Transparent);
                processedItems.Add(info);
            }

            ProcessingResult result = new ProcessingResult
            {
                Entries = processedItems,
                ProcessedInputRange = argument.Range
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
                        // TODO safer safeguard

                        // Safeguard against clearing result data during processing
                        int start = result.ProcessedInputRange.Start;
                        int count = result.ProcessedInputRange.Count;

                        if (start < data.HighlightedLogEntries.Count &&
                            start + count <= data.HighlightedLogEntries.Count)
                        {
                            for (int i = start; i < start + count; i++)
                            {
                                int highlightIndex = i - start;

                                data.HighlightedLogEntries[i].Highlight = result.Entries[highlightIndex];
                            }
                        }

                        eventBus.Send(new EntriesHighlightedEvent(start, count));
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

            var argument = new ProcessingArgument
            {
                InputEntries = data.GetFilteredLogEntries(range.Start, range.Count),
                Range = range
            };

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);
        }

        private bool ProcessQueue()
        {
            while (queue.Count > 0)
            {
                BaseQueueItem item = queue.Dequeue();

                if (item is ProcessItemsQueueItem processItems)
                {
                    if (processItems.Range.Start < 0 || processItems.Range.Start >= data.GetFilteredLogEntryCount() || processItems.Range.Start + processItems.Range.Count > data.GetFilteredLogEntryCount())
                        throw new ArgumentException("Invalid range of LogEntries to process!");

                    StartWorker(processItems.Range);
                    return true;
                }
                else if (item is ClearHighlightQueueItem)
                {
                    data.HighlightedLogEntries.Clear();

                    eventBus.Send(new HighlightedItemsClearedEvent());
                    continue;
                }
                else if (item is HandleFilteredLogEntryReplacedQueueItem replacedItem)
                {
                    if (data.HighlightedLogEntries.Count > 0)
                    {
                        if (data.HighlightedLogEntries[data.HighlightedLogEntries.Count - 1].LogEntry.LogEntryIndex > replacedItem.LogEntryIndex)
                            throw new InvalidOperationException("Replaced last LogEntry is not last item in HighlightedLogItems!");

                        if (data.HighlightedLogEntries[data.HighlightedLogEntries.Count - 1].LogEntry.LogEntryIndex == replacedItem.LogEntryIndex)
                        {
                            // Remove item and notify
                            data.HighlightedLogEntries.RemoveAt(data.HighlightedLogEntries.Count - 1);

                            eventBus.Send(new RemovedLastHighlightedLogEntryEvent(replacedItem.FilteredLogEntryIndex, replacedItem.LogEntryIndex));
                            // Item not yet exists in FilteredLogEntries - when appropriate event
                            // comes, that it has been added again, it will be processed normally.

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
            if (processedRange != null)
            {
                // Items were already processed, trying to get next chunk of data
                if (data.GetFilteredLogEntryCount() > processedRange.Start + processedRange.Count)
                {
                    int start = processedRange.Start + processedRange.Count;
                    processedRange = new Range(start, Math.Min(MAX_PROCESSED_ITEMS, data.GetFilteredLogEntryCount() - start));

                    StartWorker(processedRange);
                    return;
                }
            }
            else
            {
                if (data.GetFilteredLogEntryCount() > 0)
                {
                    processedRange = new Range(0, Math.Min(MAX_PROCESSED_ITEMS, data.GetFilteredLogEntryCount()));

                    StartWorker(processedRange);
                    return;
                }
            }
        }

        private void ContinueWork()
        {
            if (workerRunning)
                throw new InvalidOperationException("Worker cannot be running when continuing work!");

            if (!ProcessQueue())
            {
                // No task on queue, checking if there are more entries to process
                ProcessNextChunkOfData();
            }
        }

        // Public methods -----------------------------------------------------

        public LogHighlighter(EventBus eventBus, ILogHighlighterEngineDataView data)
        {
            this.eventBus = eventBus;
            this.data = data;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            queue = new Queue<BaseQueueItem>();

            eventBus.Register<AddedNewFilteredEntriesEvent>(this);
            eventBus.Register<RemovedLastFilteredLogEntryEvent>(this);
        }

        public void Receive(AddedNewFilteredEntriesEvent @event)
        {
            // Add new highlighted entries (we're on UI thread, this is safe)
            List<FilteredLogEntry> newEntries = data.GetFilteredLogEntries(@event.Start, @event.Count);
            for (int i = 0; i < newEntries.Count; i++)
            {
                data.HighlightedLogEntries.Add(new HighlightedLogEntry(newEntries[i]));
            }

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

        public void Receive(RemovedLastFilteredLogEntryEvent @event)
        {
            if (workerRunning)
            {
                if (@event.LogEntryIndex >= processedRange.Start + processedRange.Count)
                {
                    // Replaced entry will be parsed later, no need for any action
                    return;
                }
                else
                {
                    // Replaced entry is being processed right now, need to be re-processed
                    HandleFilteredLogEntryReplacedQueueItem logEntryReplacedItem = new HandleFilteredLogEntryReplacedQueueItem
                    {
                        LogEntryIndex = @event.LogEntryIndex,
                        FilteredLogEntryIndex = @event.FilteredItemIndex
                    };

                    queue.Enqueue(logEntryReplacedItem);
                }
            }
            else
            {
                // Worker is not running, re-processing entry
                HandleFilteredLogEntryReplacedQueueItem logEntryReplacedItem = new HandleFilteredLogEntryReplacedQueueItem
                {
                    LogEntryIndex = @event.LogEntryIndex
                };

                queue.Enqueue(logEntryReplacedItem);

                ContinueWork();
            }
        }
    }
}
