using LogAnalyzer.Models.Services.EventBus;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services
{
    class EventBusService : IEventBusService
    {
        private Dictionary<Type, object> listeners;

        public EventBusService()
        {
            listeners = new Dictionary<Type, object>();
        }

        public void Register<T>(IEventListener<T> listener) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            List<WeakReference<IEventListener<T>>> eventListeners;
            if (!listeners.ContainsKey(t))
            {
                eventListeners = new List<WeakReference<IEventListener<T>>>();
                listeners[t] = eventListeners;
            }
            else
            {
                eventListeners = listeners[t] as List<WeakReference<IEventListener<T>>>;
                if (eventListeners == null)
                    throw new InvalidOperationException("Invalid list type in dictionary!");
            }

            bool alreadyExists = false;
            for (int i = 0; i < eventListeners.Count; i++)
            {
                eventListeners[i].TryGetTarget(out IEventListener<T> currentListener);
                if (currentListener == listener)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                eventListeners.Add(new WeakReference<IEventListener<T>>(listener));
        }

        public void Unregister<T>(IEventListener<T> listener) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            if (!listeners.ContainsKey(t))
                return;

            List<WeakReference<IEventListener<T>>> eventListeners = listeners[t] as List<WeakReference<IEventListener<T>>>;
            for (int i = 0; i < eventListeners.Count; i++)
            {
                eventListeners[i].TryGetTarget(out IEventListener<T> currentListener);
                if (currentListener == listener)
                {
                    eventListeners.RemoveAt(i);
                    break;
                }
            }

            if (eventListeners.Count == 0)
                listeners.Remove(t);
        }

        public void Send<T>(T @event) where T : BaseEvent
        {
            Type t = typeof(T);
            if (listeners.ContainsKey(t))
            {
                List<WeakReference<IEventListener<T>>> eventListeners = listeners[t] as List<WeakReference<IEventListener<T>>>;

                int i = 0;
                while (i < eventListeners.Count)
                {
                    eventListeners[i].TryGetTarget(out IEventListener<T> listener);

                    if (listener != null)
                    {
                        listener.Receive(@event);
                        i++;
                    }
                    else
                    {
                        eventListeners.RemoveAt(i);
                    }
                }
            }
        }
    }
}
