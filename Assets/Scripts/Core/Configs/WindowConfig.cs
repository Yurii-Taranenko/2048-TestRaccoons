using System.Collections.Generic;
using UnityEngine;
using Game.Core.State;
using Game.UI.Base;

namespace Game.Core.Config
{
    /// <summary>
    /// Configuration for UI windows and HUD elements.
    /// Manages window instances and state-to-HUD mappings.
    /// </summary>
    [CreateAssetMenu(fileName = "WindowConfig", menuName = "Configs/WindowConfig")]
    public class WindowConfig : ScriptableObject
    {
        [System.Serializable]
        public class HUDMapping
        {
            public GameState state;
            public WindowBase hud;
        }

        [SerializeField] public List<WindowBase> windows = new();
        [SerializeField] public List<HUDMapping> hudMappings = new();
    }
}