using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Models.Engine;

namespace LogAnalyzer.Engine
{
    class EngineFactory : IEngineFactory
    {
        public IEngine CreateEngine(ILogSource logSource, ILogParser logParser, BaseDocument document)
        {
            return new Engine(logSource, logParser, document);
        }
    }
}
