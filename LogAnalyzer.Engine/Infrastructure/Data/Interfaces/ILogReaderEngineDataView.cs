using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Data.Interfaces
{
    interface ILogReaderEngineDataView
    {
        List<LogRecord> ResultLogEntries { get; }
        bool IsReading { get; set; }
    }
}
