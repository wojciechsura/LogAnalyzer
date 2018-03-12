using LogAnalyzer.Models.Services.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IEventBusService
    {
        void Register<T>(IEventListener<T> listener) where T : BaseEvent;
        void Unregister<T>(IEventListener<T> listener) where T : BaseEvent;
        void Send<T>(T @event) where T : BaseEvent;
    }
}
