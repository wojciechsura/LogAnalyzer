using AutoMapper;
using LogAnalyzer.Engine.Infrastructure.Data.Interfaces;
using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class LogFilter : IEventListener<AddedNewParsedEntriesEvent>, IEventListener<LastParsedEntriesItemReplacedEvent>
    {
        private readonly EventBus eventBus;
        private readonly IMapper mapper;
        private readonly ILogFilterEngineDataView data;

        public LogFilter(EventBus eventBus, IMapper mapper, ILogFilterEngineDataView data)
        {
            this.eventBus = eventBus;
            this.mapper = mapper;
            this.data = data;

            eventBus.Register<AddedNewParsedEntriesEvent>(this);
        }

        public void Receive(AddedNewParsedEntriesEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Receive(LastParsedEntriesItemReplacedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
