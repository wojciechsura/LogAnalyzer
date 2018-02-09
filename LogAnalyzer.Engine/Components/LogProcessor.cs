using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Engine.Infrastructure.Processing;
using LogAnalyzer.Engine.Interfaces;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Engine.Components
{
    class LogProcessor : IEventListener<AddedNewParsedEntriesEvent>, IEventListener<LastParsedEntriesItemReplacedEvent>
    {
        // Private constants --------------------------------------------------

        private const int MAX_FILTERED_ITEMS = 2048;
        private const int MAX_HIGHLIGHTED_ITEMS = 2048;
        private const int MAX_SEARCHED_ITEMS = 2048;

        // Private classes ----------------------------------------------------

        private abstract class BaseQueueItem
        {

        }

        private class NewDataAvailableQueueItem : BaseQueueItem
        {
            public NewDataAvailableQueueItem(int newCount)
            {
                NewCount = newCount;
            }

            public int NewCount { get; }
        }

        private class HandleLastParsedItemReplacedQueueItem : BaseQueueItem
        {
            public HandleLastParsedItemReplacedQueueItem(int metaIndex)
            {
                MetaIndex = metaIndex;
            }

            public int MetaIndex { get; }
        }

        private class ResetHighlightingQueueItem : BaseQueueItem
        {

        }

        private class ResetFilterQueueItem : BaseQueueItem
        {

        }

        private class ResetSearchQueueItem : BaseQueueItem
        {

        }

        private class FilterArgument
        {
            public FilterArgument(Range range, List<LogRecord> inputEntries, LogFilteringConfig config)
            {
                Range = range;
                InputEntries = inputEntries;
                Config = config;
            }

            public Range Range { get; }
            public List<LogRecord> InputEntries { get; }
            public LogFilteringConfig Config { get; }
        }

        private class FilterResult
        {
            public FilterResult(Range inputRange, List<LogRecord> entries)
            {
                InputRange = inputRange;
                Entries = entries;
            }

            public Range InputRange { get; }
            public List<LogRecord> Entries { get; }
        }

        private class HighlightingArgument
        {
            public HighlightingArgument(Range range, List<LogEntry> inputEntries, LogHighlighterConfig config)
            {
                Range = range;
                InputEntries = inputEntries;
                Config = config;
            }

            public Range Range { get; }
            public List<LogEntry> InputEntries { get; }
            public LogHighlighterConfig Config { get; }
        }

        private class HighlightingResult
        {
            public HighlightingResult(Range processedInputRange, List<HighlightInfo> entries)
            {
                InputRange = processedInputRange;
                Entries = entries;
            }

            public Range InputRange { get; }
            public List<HighlightInfo> Entries { get; }
        }

        private class SearchArgument
        {
            public SearchArgument(Range range, List<HighlightedLogRecord> inputEntries, LogSearchConfig config)
            {
                Range = range;
                InputEntries = inputEntries;
                Config = config;
            }

            public Range Range { get; }
            public List<HighlightedLogRecord> InputEntries { get; }
            public LogSearchConfig Config { get; }
        }

        private class SearchResult
        {
            public SearchResult(Range inputRange, List<HighlightedLogRecord> entries)
            {
                InputRange = inputRange;
                Entries = entries;
            }

            public Range InputRange { get; }
            public List<HighlightedLogRecord> Entries { get; }
        }

        private class StopData
        {
            public StopData(Action afterStop)
            {
                this.AfterStop = afterStop;
            }

            public Action AfterStop { get; set; }
        }

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

        private enum State
        {
            Working,
            Stopping,
            Stopped
        }

        // Private fields -----------------------------------------------------

        private readonly BackgroundWorker backgroundWorker;
        private readonly ILogProcessorEngineDataView data;
        private readonly EventBus eventBus;
        private readonly Queue<BaseQueueItem> queue;

        private int availableDataCount = 0;
        private int lastFilteredLogIndex = -1;
        private int lastHighlightedFilteredLogIndex = -1;
        private int lastSearchedHighlightedLogIndex = -1;

        private bool workerRunning = false;
        private State state = State.Working;
        private StopData stopData = null;

        private LogHighlighterConfig highlightConfig = null;
        private LogFilteringConfig filterConfig = null;
        private LogSearchConfig searchConfig = null;

        // Private methods ----------------------------------------------------

        private void DoStopProcessor()
        {
            queue.Clear();
            state = State.Stopped;

            stopData.AfterStop();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is FilterArgument filterArgument)
            {
                var processedItems = new List<LogRecord>();

                int index = filterArgument.Range.Start;
                for (int i = 0; i < filterArgument.InputEntries.Count; i++)
                {
                    var entry = filterArgument.InputEntries[i];

                    bool add = true;
                    if (filterArgument.Config != null)
                    {
                        add = filterArgument.Config.DefaultAction == FilterAction.Include;

                        for (int j = 0; j < filterArgument.Config.Filters.Count; j++)
                        {
                            Filter filter = filterArgument.Config.Filters[j];
                            if (filter.Predicate(entry.LogEntry))
                            {
                                add = filter.Action == FilterAction.Include;
                                break;
                            }
                        }
                    }

                    if (add)
                    {
                        processedItems.Add(entry);
                    }

                    index++;
                }

                FilterResult result = new FilterResult(filterArgument.Range, processedItems);
                e.Result = result;
            }
            else if (e.Argument is HighlightingArgument highlightArgument)
            {
                var processedItems = new List<HighlightInfo>();

                for (int i = 0; i < highlightArgument.InputEntries.Count; i++)
                {
                    var entry = highlightArgument.InputEntries[i];

                    HighlightInfo info = new HighlightInfo(Colors.Black, Colors.Transparent);

                    if (highlightArgument.Config != null)
                    {
                        for (int j = 0; j < highlightArgument.Config.Highlighters.Count; j++)
                        {
                            Highlighter highlighter = highlightArgument.Config.Highlighters[j];
                            if (highlighter.Predicate(entry))
                            {
                                info = new HighlightInfo(highlighter.Foreground, highlighter.Background);
                                break;
                            }
                        }
                    }

                    processedItems.Add(info);
                }

                HighlightingResult result = new HighlightingResult(highlightArgument.Range, processedItems);
                e.Result = result;
            }
            else if (e.Argument is SearchArgument searchArgument)
            {
                var processedEntries = new List<HighlightedLogRecord>();

                for (int i = 0; i < searchArgument.InputEntries.Count; i++)
                {
                    var entry = searchArgument.InputEntries[i];

                    if (searchArgument.Config.Predicate(entry.LogEntry))
                        processedEntries.Add(entry);
                }

                SearchResult result = new SearchResult(searchArgument.Range, processedEntries);
                e.Result = result;
            }
            else
                throw new ArgumentException("Invalid argument!");
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    if (e.Result is FilterResult filterResult)
                    {
                        int start = filterResult.InputRange.Start;
                        int count = filterResult.InputRange.Count;
                        int filteredCount = filterResult.Entries.Count;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[ F ] Filtered parsed entries - start {start}, count {count}, kept entries: {filteredCount}");
#endif

                        if (filteredCount > 0)
                        {
                            /*
                            for (int i = 0; i < filteredCount; i++)
                                data.HighlightedLogEntries.Add(new HighlightedLogRecord(filterResult.Entries[i].LogEntry, filterResult.Entries[i].Meta));
                            */

                            List<HighlightedLogRecord> records = filterResult.Entries
                                .Select(entry => new HighlightedLogRecord(entry.LogEntry, entry.Meta))
                                .ToList();
                            data.HighlightedLogEntries.AddRange(records, NotifyCollectionChangedAction.Reset);
#if DEBUG
                            System.Diagnostics.Debug.WriteLine($"[ ]->[ F ] Added {filterResult.Entries.Count} filtered entries");
#endif
                        }

                        lastFilteredLogIndex = start + count - 1;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[ F ] Last filtered log index is now {lastFilteredLogIndex}");
#endif
                    }
                    else if (e.Result is HighlightingResult highlightResult)
                    {
                        int start = highlightResult.InputRange.Start;
                        int count = highlightResult.InputRange.Count;

#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[  H] Highlighted entries - start {start}, count {count}");
#endif

                        if (highlightResult.Entries.Count != count)
                            throw new InvalidOperationException("Highlighter did not highlighted all input entries!");

                        // TODO safer safeguard (flag?)
                        // Safeguard against clearing result data during processing

                        if (start < data.HighlightedLogEntries.Count &&
                            start + count <= data.HighlightedLogEntries.Count)
                        {
                            for (int i = start; i < start + count; i++)
                            {
                                int highlightIndex = i - start;
                                data.HighlightedLogEntries[i].Highlight = highlightResult.Entries[highlightIndex];
                            }
                        }

                        lastHighlightedFilteredLogIndex = start + count - 1;

#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[  H] Last highlighted filtered log index is now {lastHighlightedFilteredLogIndex}");
#endif
                    }
                    else if (e.Result is SearchResult searchResult)
                    {
                        int start = searchResult.InputRange.Start;
                        int count = searchResult.InputRange.Count;

#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[   S] Searched entries - start {start}, count {count}");
#endif
                        if (searchResult.Entries.Count > 0)
                        {
                            for (int i = 0; i < searchResult.Entries.Count; i++)
                            {
                                data.FoundEntries.Add(searchResult.Entries[i]);
                            }

#if DEBUG
                            System.Diagnostics.Debug.WriteLine($"[ ]->[   S] Added {searchResult.Entries.Count} found entries");
#endif
                        }

                        lastSearchedHighlightedLogIndex = start + count - 1;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[   S] Last searched log index is now {lastFilteredLogIndex}");
#endif
                    }
                }
            }
            finally
            {
                workerRunning = false;

                ContinueWork();
            }
        }

        private void StartFilterWorker(Range range)
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot start worker when stopped!");

            var argument = new FilterArgument(range, data.BuildDataForFiltering(range.Start, range.Count), filterConfig);

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Started filter - start {range.Start}, count {range.Count}");
#endif
        }

        private void StartHighlighterWorker(Range range)
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot start worker when stopped!");

            var argument = new HighlightingArgument(range, data.BuildDataForHighlighting(range.Start, range.Count), highlightConfig);

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Started highlighter - start {range.Start}, count {range.Count}");
#endif
        }

        private void StartSearchWorker(Range range)
        {
            if (workerRunning)
                throw new InvalidOperationException("Cannot start already running worker!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot start worker when stopped!");

            if (searchConfig == null)
                throw new InvalidOperationException("Cannot start search worker with empty search config!");

            var argument = new SearchArgument(range, data.BuildDataForSearching(range.Start, range.Count), searchConfig);

            workerRunning = true;
            backgroundWorker.RunWorkerAsync(argument);

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Started search - start {range.Start}, count {range.Count}");
#endif
        }

        private void ProcessQueue()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot process queue when stopped or stopping!");

            if (workerRunning)
                throw new InvalidOperationException("Cannot process queue when worker is running!");

            while (queue.Count > 0)
            {
                BaseQueueItem item = queue.Dequeue();

                if (item is NewDataAvailableQueueItem newData)
                {
                    availableDataCount = newData.NewCount;

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: new items available: {availableDataCount}");
#endif
                    continue;
                }
                else if (item is HandleLastParsedItemReplacedQueueItem replacedItem)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: handling replaced item: {replacedItem.MetaIndex}");
#endif

                    var lastFilteredLog = lastFilteredLogIndex >= 0 ? data.BuildDataForFiltering(lastFilteredLogIndex, 1)[0] : null;

                    // Sanity check
                    if (lastFilteredLog != null && lastFilteredLog.Meta.Index > replacedItem.MetaIndex)
                        throw new InvalidOperationException("Replaced item is not last filtered one!");

                    if (lastFilteredLog != null && lastFilteredLog.Meta.Index == replacedItem.MetaIndex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: replaced item has already been processed");
#endif

                        // I know, that replaced item has already been processed
                        // though it might have been filtered out (or not).
                        // If it is on the filtered items list, it must be removed
                        if (data.HighlightedLogEntries.Count > 0)
                        {
                            // Sanity check
                            if (data.HighlightedLogEntries.Last().Meta.Index > replacedItem.MetaIndex)
                                throw new InvalidOperationException("Filtered item list has newer items than one replaced!");

                            if (data.HighlightedLogEntries.Last().Meta.Index == replacedItem.MetaIndex)
                            {
                                // We know, that filter included the replaced item
                                // So now it must be removed and re-filtered and re-highlighted
#if DEBUG
                                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: last filtered item matches the replaced one");
#endif
                                lastFilteredLogIndex--;

                                // If it already has been highlighted as well, it must be highlighted again
                                var lastHighlightedLog = lastHighlightedFilteredLogIndex >= 0 ? data.HighlightedLogEntries[lastHighlightedFilteredLogIndex] : null;

                                if (lastHighlightedLog != null && lastHighlightedLog.Meta.Index == replacedItem.MetaIndex)
                                    lastHighlightedFilteredLogIndex--;

                                // If it is in search results, it must be checked again
                                var lastFoundLog = lastSearchedHighlightedLogIndex >= 0 ? data.HighlightedLogEntries[lastSearchedHighlightedLogIndex] : null;

                                if (lastFoundLog != null && lastFoundLog.Meta.Index == replacedItem.MetaIndex)
                                {
                                    data.FoundEntries.RemoveAt(data.FoundEntries.Count - 1);
                                    lastSearchedHighlightedLogIndex--;
                                }

                                data.HighlightedLogEntries.RemoveAt(data.HighlightedLogEntries.Count - 1);


#if DEBUG
                                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: replaced item was removed, new last filtered index: {lastFilteredLogIndex}, new last highlighted index: {lastHighlightedFilteredLogIndex}");
#endif
                            }
                        }                     
                    }

                    // If lastFilteredLogIndex < replacedItem.Index, item was not processed yet
                    // so no action has to be done.

                    continue;
                }
                else if (item is ResetHighlightingQueueItem)
                {
                    // Re-highlight

                    foreach (var entry in data.HighlightedLogEntries)
                        entry.Highlight = null;

                    lastHighlightedFilteredLogIndex = -1;

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: Resetting highlighted items");
#endif
                    continue;
                }
                else if (item is ResetFilterQueueItem)
                {
                    // Re-filter, re-highlight, re-search

                    data.HighlightedLogEntries.Clear();
                    data.FoundEntries.Clear();
                    lastHighlightedFilteredLogIndex = -1;
                    lastFilteredLogIndex = -1;
                    lastSearchedHighlightedLogIndex = -1;

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: Resetting filtered items");
#endif
                    continue;
                }
                else if (item is ResetSearchQueueItem)
                {
                    data.FoundEntries.Clear();
                    lastSearchedHighlightedLogIndex = -1;

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Queue: Resetting found items");
#endif

                    continue;
                }
            }
        }

        private void ProcessNextChunkOfData()
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot process data when stopping or stopped!");

            if (searchConfig != null && lastSearchedHighlightedLogIndex < data.HighlightedLogEntries.Count - 1)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Found more entries to search");
#endif

                var processedRange = new Range(lastSearchedHighlightedLogIndex + 1, Math.Min(MAX_SEARCHED_ITEMS, data.HighlightedLogEntries.Count - 1 - lastSearchedHighlightedLogIndex));

                StartSearchWorker(processedRange);
            }
            if (lastHighlightedFilteredLogIndex < data.HighlightedLogEntries.Count - 1)
            {
                // Highlighting has priority over filtering
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Found more entries to highlight");
#endif

                var processedRange = new Range(lastHighlightedFilteredLogIndex + 1, Math.Min(MAX_HIGHLIGHTED_ITEMS, data.HighlightedLogEntries.Count - 1 - lastHighlightedFilteredLogIndex));

                StartHighlighterWorker(processedRange);
            }
            else if (lastFilteredLogIndex < availableDataCount - 1)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] Found more entries to filter");
#endif

                var processedRange = new Range(lastFilteredLogIndex + 1, Math.Min(MAX_FILTERED_ITEMS, availableDataCount - 1 - lastFilteredLogIndex));

                StartFilterWorker(processedRange);
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ ]->[P  ] No more data to process.");
#endif
            }
        }

        private void ContinueWork()
        {
            if (state == State.Stopping)
            {
                DoStopProcessor();
            }
            else if (state == State.Stopped)
            {
                throw new InvalidOperationException("Cannot continue work when stopped!");
            }
            else if (state == State.Working)
            {
                if (workerRunning)
                    throw new InvalidOperationException("Worker cannot be running when continuing work!");

                ProcessQueue();
                ProcessNextChunkOfData();
            }
            else
                throw new InvalidOperationException("Invalid state!");
        }

        // Public methods -----------------------------------------------------

        public LogProcessor(EventBus eventBus, ILogProcessorEngineDataView data)
        {
            this.eventBus = eventBus;
            this.data = data;

            this.queue = new Queue<BaseQueueItem>();
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            eventBus.Register<AddedNewParsedEntriesEvent>(this);
            eventBus.Register<LastParsedEntriesItemReplacedEvent>(this);
        }

        public void Stop(Action afterStop)
        {
            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Already stopping or stopped!");

            eventBus.Unregister<AddedNewParsedEntriesEvent>(this);
            eventBus.Unregister<LastParsedEntriesItemReplacedEvent>(this);

            state = State.Stopping;
            stopData = new StopData(afterStop);

            if (workerRunning)
            {
                backgroundWorker.CancelAsync();
                // RunWorkerCompleted will take care of stopping the processor.
            }
            else
            {
                DoStopProcessor();
            }
        }

        public void SetHighlighterConfig(LogHighlighterConfig newConfig)
        {
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot change config when stopped or stopping!");

            highlightConfig = newConfig;

            queue.Enqueue(new ResetHighlightingQueueItem());
            if (!workerRunning)
                ContinueWork();
        }

        public void SetFilteringConfig(LogFilteringConfig newConfig)
        {
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot change config when stopped or stopping!");

            filterConfig = newConfig;

            if (workerRunning && backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }

            queue.Enqueue(new ResetFilterQueueItem());
            if (!workerRunning)
                ContinueWork();
        }

        public void SetSearchConfig(LogSearchConfig newConfig)
        {
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot change config when stopped or stopping!");

            searchConfig = newConfig;

            queue.Enqueue(new ResetSearchQueueItem());
            if (!workerRunning)
                ContinueWork();
        }

        public void Receive(AddedNewParsedEntriesEvent @event)
        {
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot receive events when stopped!");

            availableDataCount = @event.Start + @event.Count;

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
            if (state == State.Stopped || state == State.Stopping)
                throw new InvalidOperationException("Cannot receive events when stopped!");

            if (state == State.Stopping || state == State.Stopped)
                throw new InvalidOperationException("Cannot receive events when stopped!");

            queue.Enqueue(new HandleLastParsedItemReplacedQueueItem(@event.MetaIndex));
            
            if (!workerRunning)
            {
                ContinueWork(); 
            }
        }
    }
}
