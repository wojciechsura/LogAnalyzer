using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class BookmarkEntry
    {
        public BookmarkEntry(string name, LogEntry logEntry)
        {
            Name = name;
            LogEntry = logEntry;
        }

        public string Name { get; }
        public LogEntry LogEntry { get; }
    }
}
