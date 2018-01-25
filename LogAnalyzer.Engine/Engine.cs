using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System.Collections.Concurrent;
using System.ComponentModel;
using LogAnalyzer.Models.Engine;

namespace LogAnalyzer.Engine
{
    class Engine : IEngine
    {
        public Engine(ILogSource logSource, ILogParser logParser, BaseDocument document)
        {

        }
    }
}
