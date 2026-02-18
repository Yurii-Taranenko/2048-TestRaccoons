using UnityEngine;

/// <summary>
/// Manages game state transitions and publishes state change events.
/// Prevents redundant state changes with duplicate checks.
/// </summary>
public class GameStateManager : BaseManager
{
    [SerializeField] private GameState initialState = GameState.GameScene;

    private GameState _currentState;
    public GameState CurrentState => _currentState;

    public override void Init()
    {
        EventBus.Subscribe<OnManagersInitialized>(StartGame);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnManagersInitialized>(StartGame);
    }

    /// <summary>
    /// Starts the game by setting the initial state.
    /// Called when all managers have finished initialization.
    /// </summary>
    private void StartGame(OnManagersInitialized evt)
    {
        SetState(initialState);
    }

    /// <summary>
    /// Transitions to a new game state.
    /// Ignores redundant state changes to prevent unnecessary events.
    /// </summary>
    public void SetState(GameState newState)
    {
        if (_currentState == newState)
        {
            Debug.LogWarning($"[GameStateManager] Already in state {newState}");
            return;
        }

        _currentState = newState;
        EventBus.Publish(new GameStateChangedEvent { GameState = newState });
    }
}
