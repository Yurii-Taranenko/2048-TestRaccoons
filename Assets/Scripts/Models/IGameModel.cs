/// <summary>
/// Interface for game model implementations.
/// Defines contract for score management and game data.
/// </summary>
public interface IGameModel
{
    /// <summary>
    /// Gets the current player score.
    /// </summary>
    int CurrentScore { get; }

    /// <summary>
    /// Adds points to the current score.
    /// </summary>
    /// <param name="points">Non-negative points to add</param>
    void AddScore(int points);

    /// <summary>
    /// Resets the score to zero.
    /// </summary>
    void ResetScore();
}