using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool for reusing game objects.
/// Reduces GC pressure by recycling instances instead of destroying/creating.
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly Queue<T> _pool = new();
    private readonly Transform _parent;

    /// <summary>
    /// Initializes the pool with pre-instantiated objects.
    /// </summary>
    /// <param name="prefab">Prefab to instantiate</param>
    /// <param name="initialSize">Number of objects to pre-spawn</param>
    /// <param name="parent">Optional parent transform for hierarchy organization</param>
    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Gets an available pooled object or creates a new one if pool is empty.
    /// </summary>
    public T Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(_prefab, _parent);
    }

    /// <summary>
    /// Returns an object to the pool for reuse.
    /// </summary>
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
