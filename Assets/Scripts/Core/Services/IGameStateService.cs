using Game.Core.State;

namespace Game.Core.Services
{
    public interface IGameStateService
    {
        GameState CurrentState { get; }
        void ChangeState(GameState newState);
    }
}
