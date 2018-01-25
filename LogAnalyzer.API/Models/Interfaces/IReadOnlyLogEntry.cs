using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models.Interfaces
{
    public interface IReadOnlyLogEntry
    {
        DateTime Date { get; }
        string Severity { get; }
        string Message { get; }
    }
}
