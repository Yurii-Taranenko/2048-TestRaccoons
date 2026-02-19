using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (callback == null)
        {
            Debug.LogWarning($"[EventBus] Null callback for {typeof(T).Name}");
            return;
        }

        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();

        if (_subscribers[type].Contains(callback))
        {
            Debug.LogWarning($"[EventBus] Already subscribed to {type.Name}");
            return;
        }

        _subscribers[type].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (callback == null)
            return;

        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
            _subscribers[type].Remove(callback);
    }

    public static void UnsubscribeAll<T>()
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
            _subscribers[type].Clear();
    }

    public static void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            return;

        try
        {
            var callbacks = new List<Delegate>(_subscribers[type]);
            foreach (var callback in callbacks)
            {
                try
                {
                    ((Action<T>)callback)?.Invoke(evt);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] {type.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[EventBus] Critical: {ex.Message}");
        }
    }

    public static void Clear()
    {
        _subscribers.Clear();
    }
}
