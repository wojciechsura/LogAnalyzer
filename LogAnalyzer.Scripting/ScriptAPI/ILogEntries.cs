using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Scripting.ScriptAPI
{
    public interface ILogEntries
    {
        int Count { get; }
        ILogEntry this[int index] { get; }
    }
}
