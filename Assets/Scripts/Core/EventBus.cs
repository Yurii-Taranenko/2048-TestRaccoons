using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global event bus for decoupled communication between systems.
/// Supports publish-subscribe pattern with type-safe event handling.
/// </summary>
public class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    /// <summary>
    /// Subscribes a callback to receive events of type T.
    /// Prevents duplicate subscriptions for the same callback.
    /// </summary>
    public static void Subscribe<T>(Action<T> callback)
    {
        if (callback == null)
        {
            Debug.LogWarning($"[EventBus] Cannot subscribe null callback for event {typeof(T).Name}");
            return;
        }

        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();

        // Prevent duplicate subscriptions
        if (_subscribers[type].Contains(callback))
        {
            Debug.LogWarning($"[EventBus] Callback already subscribed to {type.Name}");
            return;
        }

        _subscribers[type].Add(callback);
    }

    /// <summary>
    /// Unsubscribes a callback from receiving events of type T.
    /// </summary>
    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (callback == null)
            return;

        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Remove(callback);
        }
    }

    /// <summary>
    /// Removes all subscriptions for event type T.
    /// </summary>
    public static void UnsubscribeAll<T>()
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Clear();
        }
    }

    /// <summary>
    /// Publishes an event to all subscribed callbacks.
    /// Handles exceptions gracefully to prevent cascading failures.
    /// </summary>
    public static void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            return;

        try
        {
            // Create a copy to avoid modification during iteration
            var callbacks = new List<Delegate>(_subscribers[type]);
            foreach (var callback in callbacks)
            {
                try
                {
                    ((Action<T>)callback)?.Invoke(evt);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Error publishing {type.Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[EventBus] Critical error in Publish: {ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Clears all subscriptions. Use with caution.
    /// </summary>
    public static void Clear()
    {
        _subscribers.Clear();
    }
}
