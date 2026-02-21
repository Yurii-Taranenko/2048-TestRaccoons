using UnityEngine;
using Zenject;
using Game.Core.Services;
using Game.Game.Presenters;
using Game.Gameplay.Collision;
using Game.Gameplay.Input;
using Game.Gameplay.Boosters;
using Game.UI.HUD;

namespace Game.Core.Boot
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private CubeService _cubeService;

        public override void InstallBindings()
        {
            Container.Bind<ICubeService>().FromInstance(_cubeService).AsSingle();

            Container.BindInterfacesAndSelfTo<CubeCollisionHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<CubeInputProcessor>().AsSingle();
            Container.BindInterfacesAndSelfTo<AutoMergeBooster>().AsSingle();

            Container.BindInterfacesAndSelfTo<GamePresenter>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SceneHudConnector>().AsSingle().NonLazy();
        }
    }

    // Bridges scene-scoped booster with global HUD created in ProjectContext
    public class SceneHudConnector : IInitializable
    {
        private readonly AutoMergeBooster _booster;
        private readonly IWindowService _windowService;

        public SceneHudConnector(AutoMergeBooster booster, IWindowService windowService)
        {
            _booster = booster;
            _windowService = windowService;
        }

        public void Initialize()
        {
            if (_windowService is WindowService service)
                service.GetHud<GameHUD>()?.SetAutoMergeBooster(_booster);
        }
    }
}