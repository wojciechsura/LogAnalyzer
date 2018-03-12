using LogAnalyzer.Engine.Infrastructure.Events;
using LogAnalyzer.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Components
{
    class EngineEventBus
    {
        private Dictionary<Type, object> listeners;

        public EngineEventBus()
        {
            listeners = new Dictionary<Type, object>();
        }

        public void Register<T>(IEngineEventListener<T> listener) where T : BaseEngineEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            List<IEngineEventListener<T>> eventListeners;
            if (!listeners.ContainsKey(t))
            {
                eventListeners = new List<IEngineEventListener<T>>();
                listeners[t] = eventListeners;
            }
            else
            {
                eventListeners = listeners[t] as List<IEngineEventListener<T>>;
                if (eventListeners == null)
                    throw new InvalidOperationException("Invalid list type in dictionary!");
            }

            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);            
        }

        public void Unregister<T>(IEngineEventListener<T> listener) where T : BaseEngineEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            if (!listeners.ContainsKey(t))
                return;

            List<IEngineEventListener<T>> eventListeners = listeners[t] as List<IEngineEventListener<T>>;
            eventListeners.Remove(listener);

            if (eventListeners.Count == 0)
                listeners.Remove(t);
        }

        public void Send<T>(T @event) where T : BaseEngineEvent
        {
            Type t = typeof(T);
            if (listeners.ContainsKey(t))
            {
                List<IEngineEventListener<T>> eventListeners = listeners[t] as List<IEngineEventListener<T>>;
                foreach (var listener in eventListeners)
                    listener.Receive(@event);
            }
        }
    }
}
