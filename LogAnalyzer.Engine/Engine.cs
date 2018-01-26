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
using LogAnalyzer.Engine.Components;
using LogAnalyzer.Engine.Infrastructure.Data;
using AutoMapper;

namespace LogAnalyzer.Engine
{
    class Engine : IEngine
    {
        private readonly EventBus eventBus;
        private readonly LogReader logReader;
        private readonly IMapper mapper;
        private readonly EngineData data;

        public Engine(IMapper mapper, ILogSource logSource, ILogParser logParser, BaseDocument document)
        {
            this.mapper = mapper;
            eventBus = new EventBus();
            data = new EngineData();
            logReader = new LogReader(logSource, logParser, eventBus, mapper, data);
        }

        public void NotifySourceReady()
        {
            logReader.NotifySourceReady();
        }
    }
}
