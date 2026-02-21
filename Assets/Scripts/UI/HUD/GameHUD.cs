using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Gameplay.Boosters;
using Game.UI.Base;

namespace Game.UI.HUD
{
    public class GameHUD : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _autoMergeButton;

        private AutoMergeBooster _autoMergeBooster;

        // Injected by SceneHudConnector when gameplay scene loads
        public void SetAutoMergeBooster(AutoMergeBooster booster)
        {
            _autoMergeBooster = booster;
            if (_autoMergeButton != null)
                _autoMergeButton.interactable = booster != null;
        }

        private void OnEnable()
        {
            SetScore(0);
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);

            if (_autoMergeButton != null)
                _autoMergeButton.onClick.AddListener(OnAutoMergeClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);

            if (_autoMergeButton != null)
                _autoMergeButton.onClick.RemoveListener(OnAutoMergeClicked);
        }

        private void OnScoreChanged(ScoreChangedEvent evt) => SetScore(evt.Score);

        private void SetScore(int value)
        {
            if (_scoreText != null)
                _scoreText.text = $"Score: {value}";
        }

        private void OnAutoMergeClicked()
        {
            if (_autoMergeBooster == null || _autoMergeBooster.IsRunning) return;
            HandleAutoMergeAsync().Forget();
        }

        // Button blocks ? awaits full process ? unblocks. No callbacks.
        private async UniTaskVoid HandleAutoMergeAsync()
        {
            _autoMergeButton.interactable = false;
            await _autoMergeBooster.RunAsync();
            if (_autoMergeButton != null)
                _autoMergeButton.interactable = true;
        }
    }
}
