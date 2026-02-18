using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : WindowBase
{
    [SerializeField] private Button restartButton;

    public void Awake()
    {
        restartButton.onClick.RemoveAllListeners();

        restartButton.onClick.AddListener(() => EventBus.Publish(new OnRestartGameEvent()));
    }
}
