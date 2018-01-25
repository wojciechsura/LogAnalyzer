using LogAnalyzer.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class LogEntry : IReadOnlyLogEntry
    {
        public DateTime Date { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
    }
}
