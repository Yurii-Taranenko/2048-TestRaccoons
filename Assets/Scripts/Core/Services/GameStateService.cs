using UnityEngine;
using Zenject;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Core.State;

namespace Game.Core.Services
{
    public class GameStateService : MonoBehaviour, IGameStateService
    {
        [SerializeField] private GameState _initialState = GameState.GameScene;

        private GameState _currentState;
        public GameState CurrentState => _currentState;

        [Inject]
        private void Construct()
        {
            EventBus.Subscribe<OnManagersInitialized>(OnManagersReady);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<OnManagersInitialized>(OnManagersReady);
        }

        private void OnManagersReady(OnManagersInitialized evt)
        {
            ChangeState(_initialState);
        }

        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
                return;

            _currentState = newState;
            EventBus.Publish(new GameStateChangedEvent { GameState = newState });
        }
    }
}
