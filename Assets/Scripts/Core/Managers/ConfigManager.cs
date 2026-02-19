using UnityEngine;

/// <summary>
/// Static utility for loading ScriptableObject configurations from Resources.
/// </summary>
public static class ConfigManager
{
    private const string CONFIG_PATH = "Configs/";

    /// <summary>
    /// Loads a configuration asset from Resources/Configs/ directory.
    /// </summary>
    public static T LoadConfig<T>(string configName) where T : ScriptableObject
    {
        try
        {
            string fullPath = CONFIG_PATH + configName;
            T config = Resources.Load<T>(fullPath);

            if (config == null)
            {
                Debug.LogError($"[ConfigManager] Not found: {fullPath}");
                return null;
            }

            Debug.Log($"[ConfigManager] Loaded: {configName}");
            return config;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ConfigManager] Load error: {ex.Message}");
            return null;
        }
    }
}