using LogAnalyzer.Models.Services.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IEventListener<T> where T : BaseEvent
    {
        void Receive(T @event);
    }

}
