using UnityEngine;

namespace Game.Core.Config
{
    // Loads ScriptableObject configs from Resources/Configs/
    public static class ConfigLoader
    {
        private const string CONFIG_PATH = "Configs/";
        private static readonly System.Collections.Generic.Dictionary<string, ScriptableObject> _cache = new();

        public static T Load<T>(string configName) where T : ScriptableObject
        {
            if (_cache.TryGetValue(configName, out var cached))
                return cached as T;

            string fullPath = CONFIG_PATH + configName;
            T config = Resources.Load<T>(fullPath);

            if (config == null)
            {
                Debug.LogError($"[ConfigLoader] Config not found: {fullPath}");
                return null;
            }

            _cache[configName] = config;
            return config;
        }

        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
