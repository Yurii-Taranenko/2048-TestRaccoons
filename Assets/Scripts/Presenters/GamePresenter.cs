using System.Collections.Generic;
using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    private List<BaseManager> _managers = new List<BaseManager>();

    private WindowManager _windowManager;
    private GameStateManager _gameStateManager;
    private SceneManagerCustom _sceneManager;

    public void Init(List<BaseManager> managers)
    {
        foreach (var manager in managers)
        {
            _managers.Add(manager);
            manager.Init();
        }

        EventBus.Publish(new OnManagersInitialized());

        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<OnRestartGameEvent>(OnRestart);

        DontDestroyOnLoad(this);
    }

    private void OnGameOver(GameOverEvent evt)
    {
        _windowManager.Show<GameOverWindow>();
    }
    private void OnRestart(OnRestartGameEvent @event)
    {
        _sceneManager.RestartScene();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<OnRestartGameEvent>(OnRestart);
    }
}
