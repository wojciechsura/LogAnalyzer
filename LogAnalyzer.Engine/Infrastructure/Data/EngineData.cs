using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data
{
    class EngineData : ILogReaderEngineDataView, ILogProcessorEngineDataView, INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private List<LogEntry> parsedEntries;
        private ObservableRangeCollection<LogRecord> highlightedLogEntries;
        private ObservableRangeCollection<LogRecord> foundEntries;
        private bool isReading = false;
        private bool isProcessing = false;

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public EngineData()
        {
            parsedEntries = new List<LogEntry>();
            highlightedLogEntries = new ObservableRangeCollection<LogRecord>();
            foundEntries = new ObservableRangeCollection<LogRecord>();
        }

        public int GetLogEntryCount() => parsedEntries.Count;

        public List<LogEntry> BuildDataForFiltering(int start, int count)
        {
            if (start < 0 || start >= parsedEntries.Count || start + count > parsedEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return new List<LogEntry>(parsedEntries
                .Skip(start)
                .Take(count));
        }

        public List<LogEntry> BuildDataForHighlighting(int start, int count)
        {
            if (start < 0 || start >= highlightedLogEntries.Count || start + count > highlightedLogEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return new List<LogEntry>(highlightedLogEntries
                .Skip(start)
                .Take(count)
                .Select(he => he.LogEntry));
        }

        public List<LogRecord> BuildDataForSearching(int start, int count)
        {
            if (start < 0 || start >= highlightedLogEntries.Count || start + count > highlightedLogEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return new List<LogRecord>(highlightedLogEntries
                .Skip(start)
                .Take(count));
        }

        // Public properties --------------------------------------------------

        public List<LogEntry> ResultLogEntries => parsedEntries;

        public ObservableRangeCollection<LogRecord> HighlightedLogEntries => highlightedLogEntries;

        public ObservableRangeCollection<LogRecord> FoundEntries => foundEntries;

        public bool IsReading
        {
            get => isReading;
            set
            {
                isReading = value;
                OnPropertyChanged(nameof(IsReading));
            }
        }

        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
