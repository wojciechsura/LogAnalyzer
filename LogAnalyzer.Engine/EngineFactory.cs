using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Models.Engine;
using AutoMapper;

namespace LogAnalyzer.Engine
{
    class EngineFactory : IEngineFactory
    {
        private readonly IMapper mapper;

        public EngineFactory(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IEngine CreateEngine(ILogSource logSource, ILogParser logParser, BaseDocument document)
        {
            return new Engine(mapper, logSource, logParser, document);
        }
    }
}
