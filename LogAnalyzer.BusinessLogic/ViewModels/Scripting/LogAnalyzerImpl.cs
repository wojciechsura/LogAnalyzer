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
        private readonly Action<string> writeCallback;
        private readonly Action<string> writelnCallback;
        private ILogEntries logEntries;

        // ILogAnalyzer implementation ----------------------------------------

        void ILogAnalyzer.WritelnLog(string message)
        {
            writelnCallback?.Invoke(message);
        }

        void ILogAnalyzer.WriteLog(string message)
        {
            writeCallback?.Invoke(message);
        }

        ILogEntries ILogAnalyzer.Entries => logEntries;

        // Internal methods ---------------------------------------------------

        public LogAnalyzerImpl(IEngine engine, Action<string> writeCallback, Action<string> writelnCallback)
        {
            this.engine = engine;
            this.writeCallback = writeCallback;
            this.writelnCallback = writelnCallback;

            logEntries = new LogEntriesImpl(engine);
        }

        // Public methods -----------------------------------------------------

        public void Dispose()
        {
            this.engine = null;
        }
    }
}
