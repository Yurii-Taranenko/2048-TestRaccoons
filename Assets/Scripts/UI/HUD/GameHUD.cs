using TMPro;
using UnityEngine;

public class GameHUD : WindowBase
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    public override void Show()
    {
        base.Show();

        SetScore(1234);
    }
    public void SetScore(int value)
    {
        _scoreText.text = string.Format("Score - {0}", value);
    }
}
