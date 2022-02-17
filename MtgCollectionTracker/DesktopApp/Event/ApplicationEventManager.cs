using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DesktopApp.Event.EventModels;

namespace DesktopApp.Event
{
    internal class ApplicationEventManager : IApplicationEventManager
    {
        private static readonly IApplicationEventManager instance = new ApplicationEventManager();
        public static IApplicationEventManager Instance { get { return instance; } }

        private readonly ConcurrentDictionary<Type, List<object>> subscriptions = new();

        public void Publish<T>(T message) where T : IApplicationEvent
        {
            if (subscriptions.TryGetValue(typeof(T), out List<object> subscribers))
            {
                foreach (var subscriber in subscribers.ToArray())
                {
                    ((Action<T>)subscriber)(message);
                }
            }
        }

        public void Subscribe<T>(Action<T> action) where T : IApplicationEvent
        {
            var subscribers = subscriptions.GetOrAdd(typeof(T), t => new List<object>());
            lock (subscribers)
            {
                subscribers.Add(action);
            }
        }

        public void Unsubscribe<T>(Action<T> action) where T : IApplicationEvent
        {
            if (subscriptions.TryGetValue(typeof(T), out List<object> subscribers))
            {
                lock (subscribers)
                {
                    subscribers.Remove(action);
                }
            }
        }
    }
}
