using LogAnalyzer.Engine.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Interfaces
{
    interface IEventListener<T> where T : BaseEvent
    {
        void Receive(T @event);
    }
}
