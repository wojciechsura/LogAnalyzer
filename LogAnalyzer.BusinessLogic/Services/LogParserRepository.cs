using LineLogParser;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Services
{
    class LogParserRepository : ILogParserRepository
    {
        private List<ILogParserProvider> logParserProviders;

        public LogParserRepository()
        {
            logParserProviders = new List<ILogParserProvider>
            {
                new LineLogParserProvider()
            };

            // Verifying uniqueness of names
            if (logParserProviders.Any(p => logParserProviders.Any(q => q.UniqueName == p.UniqueName && p != q)))
                throw new InvalidOperationException("Not all log source providers names are unique!");
        }

        public IEnumerable<ILogParserProvider> LogParserProviders => logParserProviders;
    }
}
