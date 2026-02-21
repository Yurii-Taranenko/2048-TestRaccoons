using UnityEngine;

namespace Game.UI.Base
{
    /// <summary>
    /// Base class for all UI windows and HUDs.
    /// Provides common show/hide functionality.
    /// </summary>
    public abstract class WindowBase : MonoBehaviour, IWindow
    {
        /// <summary>
        /// Show this window by activating its GameObject.
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide this window by deactivating its GameObject.
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
