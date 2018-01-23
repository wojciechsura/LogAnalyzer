using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;

namespace FileLogSource
{
    class FileLogSource : ILogSource
    {
        private ILogSourceConfiguration configuration;

        public FileLogSource(ILogSourceConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}
