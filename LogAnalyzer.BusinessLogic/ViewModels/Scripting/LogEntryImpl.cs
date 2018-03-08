using System;
using System.Collections.Generic;
using LogAnalyzer.API.Models;
using LogAnalyzer.Scripting.ScriptAPI;

namespace LogAnalyzer.BusinessLogic.ViewModels.Scripting
{
    internal class LogEntryImpl : ILogEntry
    {
        // Private fields -----------------------------------------------------

        private LogEntry logEntry;
        private List<BaseColumnInfo> columns;

        // ILogEntry implementation -------------------------------------------

        int ILogEntry.Index => logEntry.Index;

        DateTime ILogEntry.Date => logEntry.Date;

        string ILogEntry.Severity => logEntry.Severity;

        string ILogEntry.Message => logEntry.Message;

        string ILogEntry.Custom(string name)
        {
            throw new NotImplementedException();
        }

        // Internal methods ---------------------------------------------------

        LogEntryImpl(LogEntry logEntry, List<BaseColumnInfo> columns)
        {
            this.logEntry = logEntry;
            this.columns = columns;
        }
    }
}