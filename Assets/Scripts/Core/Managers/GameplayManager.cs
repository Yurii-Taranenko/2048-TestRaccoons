using UnityEngine;

/// <summary>
/// Manages gameplay configuration and settings.
/// Provides centralized access to game mechanics parameters.
/// </summary>
public class GameplayManager : BaseManager
{
    private GameplayConfig _config;

    public GameplayConfig Config => _config;

    /// <summary>
    /// Sets the configuration for gameplay systems.
    /// Must be called before Init().
    /// </summary>
    public void SetConfig(GameplayConfig config)
    {
        _config = config;
    }

    public override void Init()
    {
        if (_config == null)
        {
            Debug.LogError("[GameplayManager] Config not assigned!");
            return;
        }

        Debug.Log("[GameplayManager] Initialized with config");
    }
}