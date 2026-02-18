using TMPro;
using UnityEngine;

/// <summary>
/// Displays the current game score in the HUD.
/// Listens to score change events from the Model.
/// </summary>
public class GameHUD : WindowBase
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    /// <summary>
    /// Updates the score display when score changes.
    /// </summary>
    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        SetScore(evt.Score);
    }

    private void SetScore(int value)
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {value}";
    }
}
