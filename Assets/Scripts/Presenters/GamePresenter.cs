using System.Collections.Generic;
using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    private List<BaseManager> _managers = new List<BaseManager>();

    private WindowManager _windowManager;
    private GameStateManager _gameStateManager;
    private SceneManagerCustom _sceneManager;
    private GameModel _gameModel;

    /// <summary>
    /// Initializes all systems and establishes event subscriptions.
    /// </summary>
    /// <param name="managers">List of manager systems to initialize</param>
    /// <param name="gameModel">Model instance for game data</param>
    public void Init(List<BaseManager> managers, GameModel gameModel)
    {
        _gameModel = gameModel;
        CacheManagers(managers);
        InitializeManagers();
        InitializeModel();
        PublishInitializationEvent();
        SubscribeToEvents();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Extracts specific manager instances for direct access.
    /// </summary>
    private void CacheManagers(List<BaseManager> managers)
    {
        foreach (var manager in managers)
        {
            _managers.Add(manager);

            if (manager is WindowManager windowManager)
                _windowManager = windowManager;
            else if (manager is GameStateManager gameStateManager)
                _gameStateManager = gameStateManager;
            else if (manager is SceneManagerCustom sceneManager)
                _sceneManager = sceneManager;
        }

        ValidateMandatoryManagers();
    }

    /// <summary>
    /// Ensures all required managers are present.
    /// Logs errors for debugging if dependencies are missing.
    /// </summary>
    private void ValidateMandatoryManagers()
    {
        if (_windowManager == null)
            Debug.LogError("[GamePresenter] WindowManager not found in managers list!");
        if (_gameStateManager == null)
            Debug.LogError("[GamePresenter] GameStateManager not found in managers list!");
        if (_sceneManager == null)
            Debug.LogError("[GamePresenter] SceneManagerCustom not found in managers list!");
        if (_gameModel == null)
            Debug.LogError("[GamePresenter] GameModel not found or not passed!");
    }

    private void InitializeManagers()
    {
        foreach (var manager in _managers)
            manager.Init();
    }

    private void InitializeModel()
    {
        _gameModel?.Initialize();
    }

    /// <summary>
    /// Signals to all listeners that initialization is complete.
    /// </summary>
    private void PublishInitializationEvent()
    {
        EventBus.Publish(new OnManagersInitialized());
    }

    /// <summary>
    /// Subscribes to critical gameplay events.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<OnRestartGameEvent>(OnRestart);
    }

    private void OnGameOver(GameOverEvent evt)
    {
        _windowManager?.Show<GameOverWindow>();
    }

    private void OnRestart(OnRestartGameEvent @event)
    {
        _gameModel?.ResetScore();
        _sceneManager?.RestartScene();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<OnRestartGameEvent>(OnRestart);
    }
}
