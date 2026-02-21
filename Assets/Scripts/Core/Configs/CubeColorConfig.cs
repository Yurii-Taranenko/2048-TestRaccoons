using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Config
{
    [CreateAssetMenu(fileName = "CubeColorConfig", menuName = "Configs/CubeColorConfig")]
    public class CubeColorConfig : ScriptableObject
    {
        [System.Serializable]
        public class CubeColorEntry
        {
            public int value;
            public Color color;
        }

        [SerializeField] private List<CubeColorEntry> colorMap = new()
        {
            new() { value = 2, color = new Color(0.8f, 0.8f, 0.8f) },
            new() { value = 4, color = new Color(0.7f, 0.7f, 0.7f) },
            new() { value = 8, color = new Color(1f, 0.8f, 0.6f) },
            new() { value = 16, color = new Color(1f, 0.6f, 0.4f) },
            new() { value = 32, color = new Color(1f, 0.4f, 0.2f) },
            new() { value = 64, color = new Color(1f, 0.2f, 0f) },
            new() { value = 128, color = new Color(1f, 0f, 0f) },
            new() { value = 256, color = new Color(1f, 0.6f, 0f) },
            new() { value = 512, color = new Color(1f, 0.8f, 0f) },
            new() { value = 1024, color = new Color(0f, 0.8f, 1f) },
            new() { value = 2048, color = new Color(0f, 0.6f, 1f) },
        };

        private Dictionary<int, Color> _colorCache;

        public Color GetColorForValue(int value)
        {
            if (_colorCache == null)
            {
                _colorCache = new Dictionary<int, Color>();
                foreach (var entry in colorMap)
                    _colorCache[entry.value] = entry.color;
            }

            if (_colorCache.TryGetValue(value, out var color))
                return color;

            return Color.white;
        }
    }
}