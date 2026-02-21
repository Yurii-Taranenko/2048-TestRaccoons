using UnityEngine;
using UnityEngine.UI;
using Game.Core.Events;
using Game.Core.Utils;
using Game.UI.Base;

namespace Game.UI.Windows
{
    public class GameOverWindow : WindowBase
    {
        [SerializeField] private Button restartButton;
        private bool _isInitialized;

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                if (restartButton != null)
                    restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        private void OnDisable()
        {
            if (_isInitialized)
            {
                _isInitialized = false;
                if (restartButton != null)
                    restartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        private void OnRestartClicked()
        {
            EventBus.Publish(new OnRestartGameEvent());
        }
    }
}
