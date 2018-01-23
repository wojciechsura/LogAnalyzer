using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;

namespace LogAnalyzer.Engine
{
    class Engine : IEngine
    {
        private ILogSource logSource;
        private ILogParser logParser;

        public Engine(ILogSource logSource, ILogParser logParser)
        {
            this.logSource = logSource;
            this.logParser = logParser;
        }
    }
}
