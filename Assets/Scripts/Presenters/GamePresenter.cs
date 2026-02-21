using System;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Core.State;
using Game.Core.Services;
using Game.UI.Windows;

namespace Game.Game.Presenters
{
    // MVP Presenter — bridges model, UI and game flow
    public class GamePresenter : IDisposable
    {
        private readonly IScoreService _scoreService;
        private readonly IWindowService _windowService;
        private readonly ISceneService _sceneService;
        private readonly ICubeService _cubeService;
        private readonly IGameStateService _gameStateService;

        public GamePresenter(
            IScoreService scoreService,
            IWindowService windowService,
            ISceneService sceneService,
            ICubeService cubeService,
            IGameStateService gameStateService)
        {
            _scoreService = scoreService;
            _windowService = windowService;
            _sceneService = sceneService;
            _cubeService = cubeService;
            _gameStateService = gameStateService;

            Initialize();
        }

        public void Initialize()
        {
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
            EventBus.Subscribe<OnRestartGameEvent>(OnRestart);
        }

        private void OnGameOver(GameOverEvent evt)
        {
            if (_cubeService == null)
                return;

            // Skip game over for the active (not yet launched) cube
            var activeCube = _cubeService.ActiveCube;
            if (activeCube != null && evt.CubeId == activeCube.Id)
                return;

            _windowService?.Show<GameOverWindow>();
            _gameStateService?.ChangeState(GameState.GameOver);
        }

        private void OnRestart(OnRestartGameEvent evt)
        {
            _scoreService?.ResetScore();
            _cubeService?.ResetCubes();
            _windowService?.HideAllWindows();
            // Restore state — WindowService picks up GameStateChangedEvent and shows HUD
            _gameStateService?.ChangeState(GameState.GameScene);
            _sceneService?.RestartScene();
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            EventBus.Unsubscribe<OnRestartGameEvent>(OnRestart);
        }
    }
}
