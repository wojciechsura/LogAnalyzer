using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class ProfilingEntry
    {
        public ProfilingEntry(LogEntry entry)
        {
            Entry = entry;
        }

        public LogEntry Entry { get; }
        public int Step { get; set; } = 0;
        public TimeSpan FromPrevious { get; set; } = TimeSpan.Zero;
        public TimeSpan FromStart { get; set; } = TimeSpan.Zero;
    }
}
