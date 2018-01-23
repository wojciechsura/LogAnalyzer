using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class BaseDocument
    {
        private readonly ObservableCollection<LogRecord> logRecords;

        public BaseDocument()
        {
            logRecords = new ObservableCollection<LogRecord>();
        }

        public ObservableCollection<LogRecord> LogRecords => logRecords;
    }
}
