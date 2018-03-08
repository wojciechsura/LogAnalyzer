using LogAnalyzer.Scripting.ScriptAPI;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Scripting
{
    class LogEntriesImpl : ILogEntries
    {
        // Private fields -----------------------------------------------------

        private readonly IEngine engine;

        // Internal methods ---------------------------------------------------

        public LogEntriesImpl(IEngine engine)
        {
            this.engine = engine;
        }

        // ILogEntries implementation -----------------------------------------

        ILogEntry ILogEntries.this[int index] => new LogEntryImpl(engine.LogEntries[index], engine.GetColumnInfos());

        int ILogEntries.Count => engine.LogEntries.Count;
    }
}
