using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bootstrap coordinator that initializes all game managers and systems.
/// Validates configuration before startup and injects dependencies.
/// For production, we can use a DI-container framework or service locator pattern. But for this simple project, i will just manually inject dependencies (but only once on start).
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private List<BaseManager> _managers;
    [SerializeField] private GamePresenter _gamePresenter;

    [Header("Configs")]
    [SerializeField] private WindowConfig _windowConfig;
    [SerializeField] private GameplayConfig _gameplayConfig;

    private void Awake()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        LoadConfigs();
        InjectConfigs();

        GameModel gameModel = new GameModel();
        _gamePresenter.Init(_managers, gameModel);

        Debug.Log("[GameBootstrap] Game initialized successfully");
    }

    /// <summary>
    /// Validates that all required dependencies are assigned.
    /// Prevents runtime errors by catching configuration issues early.
    /// </summary>
    private bool ValidateSetup()
    {
        bool isValid = true;

        if (_managers == null || _managers.Count == 0)
        {
            Debug.LogError("[GameBootstrap] CRITICAL: Managers list is empty!");
            isValid = false;
        }

        if (_gamePresenter == null)
        {
            Debug.LogError("[GameBootstrap] CRITICAL: GamePresenter is not assigned!");
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Loads configuration files from Resources.
    /// Falls back to Resources.Load if configs are not assigned in Inspector.
    /// </summary>
    private void LoadConfigs()
    {
        if (_windowConfig == null)
        {
            _windowConfig = ConfigManager.LoadConfig<WindowConfig>("WindowConfig");
            if (_windowConfig == null)
                Debug.LogError("[GameBootstrap] WindowConfig not found in Assets/Resources/Configs/!");
        }

        if (_gameplayConfig == null)
        {
            _gameplayConfig = ConfigManager.LoadConfig<GameplayConfig>("GameplayConfig");
            if (_gameplayConfig == null)
                Debug.LogError("[GameBootstrap] GameplayConfig not found in Assets/Resources/Configs/!");
        }
    }

    /// <summary>
    /// Injects configuration into respective managers.
    /// Uses type checking to route configs to appropriate systems.
    /// </summary>
    private void InjectConfigs()
    {
        foreach (var manager in _managers)
        {
            if (manager is WindowManager windowManager && _windowConfig != null)
                windowManager.SetConfig(_windowConfig);
            else if (manager is GameplayManager gameplayManager && _gameplayConfig != null)
                gameplayManager.SetConfig(_gameplayConfig);
        }
    }
}
