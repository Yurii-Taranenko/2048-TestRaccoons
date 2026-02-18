using UnityEngine;

/// <summary>
/// Model layer responsible for managing game data and score.
/// Publishes events when state changes to decouple from UI.
/// </summary>
public class GameModel : IGameModel
{
    private int _currentScore;
    public int CurrentScore => _currentScore;

    /// <summary>
    /// Initializes the model state without publishing events.
    /// Events should only be published during gameplay, not during initialization.
    /// </summary>
    public void Initialize()
    {
        _currentScore = 0;
        Debug.Log("[GameModel] Initialized with score: 0");
    }

    /// <summary>
    /// Adds points to the current score and notifies listeners.
    /// </summary>
    /// <param name="points">Points to add (must be non-negative)</param>
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
}