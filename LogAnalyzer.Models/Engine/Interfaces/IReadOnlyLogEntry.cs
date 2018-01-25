using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine.Interfaces
{
    interface IReadOnlyLogEntry
    {
        DateTime Date { get; }
        string Severity { get; }
        string Message { get; }
    }
}
