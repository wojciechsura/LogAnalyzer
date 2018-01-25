using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class LogReader
    {
        private class ProcessingResult
        {
            public bool Changed { get; set; }
            public int? ReplacedItemIndex { get; set; }
        }

        private readonly ILogSource logSource;
        private readonly ILogParser logParser;
        private readonly EventBus eventBus;
        private readonly ILogReaderEngineDataView data;

        private readonly BackgroundWorker backgroundWorker;

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        public LogReader(ILogSource logSource, ILogParser logParser, EventBus eventBus, ILogReaderEngineDataView data)
        {
            this.logSource = logSource ?? throw new ArgumentNullException(nameof(logSource));
            this.logParser = logParser ?? throw new ArgumentNullException(nameof(logParser));
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.data = data ?? throw new ArgumentNullException(nameof(data));

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }
    }
}
