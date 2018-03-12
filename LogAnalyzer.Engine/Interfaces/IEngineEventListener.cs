using LogAnalyzer.Engine.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Interfaces
{
    interface IEngineEventListener<T> where T : BaseEngineEvent
    {
        void Receive(T @event);
    }
}
