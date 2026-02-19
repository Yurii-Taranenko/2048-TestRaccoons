using System.Collections.Generic;
using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    private List<BaseManager> _managers = new List<BaseManager>();
    private WindowManager _windowManager;
    private GameStateManager _gameStateManager;
    private SceneManagerCustom _sceneManager;
    private CubeManager _cubeManager;
    private GameModel _gameModel;

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
            else if (manager is CubeManager cubeManager)
                _cubeManager = cubeManager;
        }
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

    private void PublishInitializationEvent()
    {
        EventBus.Publish(new OnManagersInitialized());
    }

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
        _gameModel?.Dispose();
    }
}
