using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Utils
{
    // Global event bus using struct events
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public static void Subscribe<T>(Action<T> callback) where T : struct
        {
            if (callback == null)
                return;

            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();

            if (_subscribers[type].Contains(callback))
                return;

            _subscribers[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            if (callback == null)
                return;

            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
                _subscribers[type].Remove(callback);
        }

        public static void UnsubscribeAll<T>() where T : struct
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
                _subscribers[type].Clear();
        }

        public static void Publish<T>(T evt) where T : struct
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                return;

            // Copy to protect against modification during iteration
            var callbacks = new List<Delegate>(_subscribers[type]);
            foreach (var callback in callbacks)
            {
                try
                {
                    ((Action<T>)callback)?.Invoke(evt);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Error in {type.Name} handler: {ex.Message}");
                }
            }
        }

        public static void Clear()
        {
            _subscribers.Clear();
        }
    }
}
