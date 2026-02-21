using Game.Core.State;
using Game.UI.Base;

namespace Game.Core.Services
{
    public interface IWindowService
    {
        void Show<T>() where T : WindowBase;
        void HideAllWindows();
        void HideAllHuds();
        void ForceOpenHUD(GameState state);
    }
}
