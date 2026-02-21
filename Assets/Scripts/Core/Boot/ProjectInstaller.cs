using UnityEngine;
using Zenject;
using Game.Core.Config;
using Game.Core.Services;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Game.Models;

namespace Game.Core.Boot
{
    // Global container — persists across scenes
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private GameplayConfig _gameplayConfig;
        [SerializeField] private CubeColorConfig _cubeColorConfig;
        [SerializeField] private WindowConfig _windowConfig;

        [Header("Global Services")]
        [SerializeField] private InputService _inputService;
        [SerializeField] private WindowService _windowService;
        [SerializeField] private GameStateService _gameStateService;
        [SerializeField] private SceneService _sceneService;

        public override void InstallBindings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            BindConfigs();
            BindGlobalServices();
            BindModels();

            Container.BindInterfacesTo<ProjectInitializer>().AsSingle().NonLazy();
        }

        private void BindConfigs()
        {
            // Fallback to Resources if not assigned in inspector
            if (_gameplayConfig == null)
                _gameplayConfig = ConfigLoader.Load<GameplayConfig>("GameplayConfig");
            if (_cubeColorConfig == null)
                _cubeColorConfig = ConfigLoader.Load<CubeColorConfig>("CubeColorConfig");
            if (_windowConfig == null)
                _windowConfig = ConfigLoader.Load<WindowConfig>("WindowConfig");

            Container.BindInstance(_gameplayConfig).AsSingle();
            Container.BindInstance(_cubeColorConfig).AsSingle();
            Container.BindInstance(_windowConfig).AsSingle();
        }

        private void BindGlobalServices()
        {
            Container.Bind<IInputService>().FromInstance(_inputService).AsSingle();
            Container.Bind<IWindowService>().FromInstance(_windowService).AsSingle();
            Container.Bind<IGameStateService>().FromInstance(_gameStateService).AsSingle();
            Container.Bind<ISceneService>().FromInstance(_sceneService).AsSingle();
        }

        private void BindModels()
        {
            Container.Bind<IScoreService>().To<GameModel>().AsSingle();
        }
    }

    // Entry point — initializes models and starts the loading chain
    public class ProjectInitializer : IInitializable
    {
        private readonly IScoreService _scoreService;

        public ProjectInitializer(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        public void Initialize()
        {
            if (_scoreService is GameModel model)
                model.Initialize();

            EventBus.Publish(new OnManagersInitialized());
        }
    }
}