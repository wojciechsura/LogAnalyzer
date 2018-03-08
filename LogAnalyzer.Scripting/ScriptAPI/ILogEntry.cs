using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Scripting.ScriptAPI
{
    public interface ILogEntry
    {
        string Custom(string name);

        int Index { get; }
        DateTime Date { get; }
        string Severity { get; }
        string Message { get; }
    }
}
