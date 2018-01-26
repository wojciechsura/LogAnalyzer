using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data
{
    class EngineData : ILogReaderEngineDataView, ILogFilterEngineDataView
    {
        private List<LogEntry> parsedEntries;
        private List<FilteredLogEntry> filteredLogEntries;
        private IMapper mapper;

        public EngineData(IMapper mapper)
        {
            this.mapper = mapper;
            parsedEntries = new List<LogEntry>();
            filteredLogEntries = new List<FilteredLogEntry>();
        }

        public int GetLogEntryCount() => parsedEntries.Count;

        public List<LogEntry> GetLogEntries(int start, int count)
        {
            if (start < 0 || start >= parsedEntries.Count || start + count > parsedEntries.Count)
                throw new ArgumentException("Invalid start/count combination!");

            return mapper.Map<List<LogEntry>>(parsedEntries
                .Skip(start)
                .Take(count));
        }

        public List<LogEntry> ResultLogEntries => parsedEntries;

        public List<FilteredLogEntry> FilteredLogEntries => filteredLogEntries;
    }
}
