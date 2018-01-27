using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data
{
    class EngineData : ILogReaderEngineDataView, ILogFilterEngineDataView, ILogHighlighterEngineDataView
    {
        // Private fields -----------------------------------------------------

        private List<LogEntry> parsedEntries;
        private List<FilteredLogEntry> filteredLogEntries;
        private ObservableRangeCollection<HighlightedLogEntry> highlightedLogEntries;

        // Public methods -----------------------------------------------------

        public EngineData()
        {
            parsedEntries = new List<LogEntry>();
            filteredLogEntries = new List<FilteredLogEntry>();
            highlightedLogEntries = new ObservableRangeCollection<HighlightedLogEntry>();
        }

        public int GetLogEntryCount() => parsedEntries.Count;

        public List<LogEntry> GetLogEntries(int start, int count)
        {
            if (start < 0 || start >= parsedEntries.Count || start + count > parsedEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return new List<LogEntry>(parsedEntries                
                .Skip(start)
                .Take(count));
        }

        public int GetFilteredLogEntryCount() => filteredLogEntries.Count;

        public List<FilteredLogEntry> GetFilteredLogEntries(int start, int count)
        {
            if (start < 0 || start >= filteredLogEntries.Count || start + count > filteredLogEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return new List<FilteredLogEntry>(filteredLogEntries
                .Skip(start)
                .Take(count));
        }

        // Public properties --------------------------------------------------

        public List<LogEntry> ResultLogEntries => parsedEntries;

        public List<FilteredLogEntry> FilteredLogEntries => filteredLogEntries;

        public ObservableRangeCollection<HighlightedLogEntry> HighlightedLogEntries => highlightedLogEntries;
    }
}
