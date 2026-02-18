using System;
using UnityEngine;

public class GameStateManager : BaseManager
{
    [System.Serializable]
    public class HUDMapping
    {
        public GameState state;
        public WindowBase hud;
    }

    [SerializeField] private GameState initialState = GameState.GameScene;

    private GameState _currentState;
    public GameState CurrentState => _currentState;

    public override void Init()
    {
        EventBus.Subscribe<OnManagersInitialized>(StartGame);
    }

    public void StartGame(OnManagersInitialized evt)
    {
        SetState(initialState);
    }

    public void SetState(GameState newState)
    {
        _currentState = newState;
        EventBus.Publish(new GameStateChangedEvent { GameState = newState });
    }
}
