using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class FilteredLogEntry
    {
        public int LogEntryIndex { get; set; }

        public DateTime Date { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
    }
}
