using UnityEngine;

/// <summary>
/// Model layer responsible for managing game data and score.
/// </summary>
public class GameModel : IGameModel
{
    private int _currentScore;
    public int CurrentScore => _currentScore;

    /// <summary>
    /// Initializes the model state without publishing events.
    /// </summary>
    public void Initialize()
    {
        _currentScore = 0;
        Debug.Log("[GameModel] Initialized with score: 0");
        SubscribeToGameEvents();
    }

    /// <summary>
    /// Subscribes to gameplay events that affect score.
    /// </summary>
    private void SubscribeToGameEvents()
    {
        EventBus.Subscribe<CubeMergedEvent>(OnCubeMerged);
    }

    /// <summary>
    /// Handles cube merge events and updates score.
    /// </summary>
    private void OnCubeMerged(CubeMergedEvent evt)
    {
        AddScore(evt.ScoreGained);
    }

    /// <summary>
    /// Adds points to the current score and notifies listeners.
    /// </summary>
    public void AddScore(int points)
    {
        if (points < 0)
        {
            Debug.LogWarning($"[GameModel] Attempt to add negative score: {points}");
            return;
        }

        _currentScore += points;
        EventBus.Publish(new ScoreChangedEvent { Score = _currentScore });
    }

    /// <summary>
    /// Resets the score to zero and notifies listeners.
    /// </summary>
    public void ResetScore()
    {
        _currentScore = 0;
        EventBus.Publish(new ScoreChangedEvent { Score = _currentScore });
    }

    /// <summary>
    /// Cleanup on destruction.
    /// </summary>
    public void Dispose()
    {
        EventBus.Unsubscribe<CubeMergedEvent>(OnCubeMerged);
    }
}