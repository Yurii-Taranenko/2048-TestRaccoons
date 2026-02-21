using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Utils
{
    // Simple pool for reusing MonoBehaviour objects
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Queue<T> _pool = new();
        private readonly Transform _parent;

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

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        public void Reset()
        {
            while (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                Object.Destroy(obj.gameObject);
            }
            _pool.Clear();
        }
    }
}
