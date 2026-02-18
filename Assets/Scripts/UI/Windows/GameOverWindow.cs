using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles game over UI window and restart functionality.
/// Manages button listeners without creating lambda memory leaks.
/// </summary>
public class GameOverWindow : WindowBase
{
    [SerializeField] private Button restartButton;
    private bool _isInitialized;

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void OnDisable()
    {
        if (_isInitialized)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }

    /// <summary>
    /// Publishes restart event when button is clicked.
    /// Uses named method reference to avoid lambda memory leaks.
    /// </summary>
    private void OnRestartClicked()
    {
        EventBus.Publish(new OnRestartGameEvent());
    }
}
