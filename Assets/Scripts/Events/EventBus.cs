using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class EventBus
    {
        private static EventBus _instance;
        public static EventBus Instance 
        {
            get
            {
                if (_instance == null)
                    _instance = new EventBus();
                return _instance;
            }
        }
        public static void Reset()
        {
            _instance = null;
        }
        private readonly Dictionary<Type, Delegate> EventTable = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, List<string>> DebugSubscribers = new Dictionary<Type, List<string>>();

        public void Subscribe<T>(Action<T> handler, string subscriberName = null) where T : struct
        {
            var eventType = typeof(T);

            if (!string.IsNullOrEmpty(subscriberName))
            {
                if (!DebugSubscribers.ContainsKey(eventType))
                    DebugSubscribers[eventType] = new List<string>();
                DebugSubscribers[eventType].Add(subscriberName);
            }

            if (!EventTable.TryAdd(eventType, handler))
            {
                EventTable[eventType] = Delegate.Combine(EventTable[eventType], handler);
            }
#if UNITY_EDITOR
            Debug.Log($"[EventBus] {subscriberName ?? ""} subscribed to {eventType.Name}");
#endif
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var eventType = typeof(T);
            if (EventTable.ContainsKey(eventType))
            {
                var currentDelegate = EventTable[eventType];
                var newDelegate = Delegate.Remove(currentDelegate, handler);

                if (newDelegate == null)
                {
                    EventTable.Remove(eventType);
                    DebugSubscribers.Remove(eventType);
                }
                else
                {
                    EventTable[eventType] = newDelegate;
                }
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            var eventType = typeof(T);
            if (EventTable.ContainsKey(eventType))
            {
                var handler = EventTable[eventType] as Action<T>;
                try
                {
                    handler?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}