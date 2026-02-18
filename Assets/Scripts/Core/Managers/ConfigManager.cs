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
                Debug.LogError($"[ConfigManager] Config not found: {fullPath}. " +
                    $"Make sure the file is located at: Assets/Resources/{fullPath}.asset");
                return null;
            }

            Debug.Log($"[ConfigManager] Successfully loaded config: {configName}");
            return config;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ConfigManager] Error loading config {configName}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads configuration or creates a default instance if file not found.
    /// </summary>
    public static T LoadConfigOrDefault<T>(string configName) where T : ScriptableObject
    {
        T config = LoadConfig<T>(configName);
        if (config == null)
        {
            Debug.LogWarning($"[ConfigManager] Creating default config for {configName}");
            config = ScriptableObject.CreateInstance<T>();
        }
        return config;
    }
}