using LogAnalyzer.Scripting.ScriptAPI;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Scripting
{
    class LogEventArgs : EventArgs
    {
        public string Message { get; }

        public LogEventArgs(string message)
        {
            Message = message;
        }
    }

    delegate void LogMessageEventHandler(object sender, LogEventArgs args);

    class LogAnalyzerImpl : ILogAnalyzer, IDisposable
    {
        // Private fields -----------------------------------------------------

        private IEngine engine;
        private ILogEntries logEntries;

        // ILogAnalyzer implementation ----------------------------------------

        void ILogAnalyzer.WritelnLog(string message)
        {
            this.WritelnLog?.Invoke(this, new LogEventArgs(message));
        }

        void ILogAnalyzer.WriteLog(string message)
        {
            this.WriteLog?.Invoke(this, new LogEventArgs(message));
        }

        ILogEntries ILogAnalyzer.Entries => logEntries;

        // Internal methods ---------------------------------------------------

        public LogAnalyzerImpl(IEngine engine)
        {
            this.engine = engine;
            logEntries = new LogEntriesImpl(engine);
        }

        // Internal events ----------------------------------------------------

        internal event LogMessageEventHandler WriteLog;
        internal event LogMessageEventHandler WritelnLog;

        // Public methods -----------------------------------------------------

        public void Dispose()
        {
            this.engine = null;
        }
    }
}
