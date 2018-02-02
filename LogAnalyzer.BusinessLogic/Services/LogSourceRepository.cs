using FileLogSource;
using FilesLogSource;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Services
{
    class LogSourceRepository : ILogSourceRepository
    {
        private List<ILogSourceProvider> logSourceProviders;

        public LogSourceRepository()
        {
            logSourceProviders = new List<ILogSourceProvider>
            {
                new FileLogSourceProvider(),
                new FilesLogSourceProvider()
            };

            // Verifying uniqueness of names
            if (logSourceProviders.Any(p => logSourceProviders.Any(q => q.UniqueName == p.UniqueName && p != q)))
                throw new InvalidOperationException("Not all log source providers names are unique!");
        }

        public IEnumerable<ILogSourceProvider> LogSourceProviders => logSourceProviders;
    }
}
