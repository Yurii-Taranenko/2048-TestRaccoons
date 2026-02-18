using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages UI window and HUD display based on game state.
/// Caches window instances for rapid access.
/// </summary>
public class WindowManager : BaseManager
{
    private WindowConfig _config;
    private Dictionary<Type, WindowBase> _windowCache;
    private Dictionary<GameState, WindowBase> _hudCache;

    /// <summary>
    /// Sets the configuration for this manager.
    /// Must be called before Init().
    /// </summary>
    public void SetConfig(WindowConfig config)
    {
        _config = config;
    }

    public override void Init()
    {
        if (_config == null)
        {
            Debug.LogError("[WindowManager] Config not assigned! GameBootstrap should call SetConfig()");
            return;
        }

        InitializeWindowCache();
        InitializeHudCache();
        EventBus.Subscribe<GameStateChangedEvent>(UpdateHUD);
    }

    private void InitializeWindowCache()
    {
        _windowCache = new Dictionary<Type, WindowBase>();
        if (_config?.windows == null)
        {
            Debug.LogWarning("[WindowManager] Windows list is empty or config is null");
            return;
        }

        foreach (var window in _config.windows)
        {
            if (window != null)
                _windowCache[window.GetType()] = window;
        }
    }

    private void InitializeHudCache()
    {
        _hudCache = new Dictionary<GameState, WindowBase>();
        if (_config?.hudMappings == null)
        {
            Debug.LogWarning("[WindowManager] HUD mappings is empty or config is null");
            return;
        }

        foreach (var mapping in _config.hudMappings)
        {
            if (mapping?.hud != null)
                _hudCache[mapping.state] = mapping.hud;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(UpdateHUD);
    }

    public void UpdateHUD(GameStateChangedEvent evt)
    {
        HideAllHud();

        if (_hudCache.TryGetValue(evt.GameState, out var hud))
        {
            hud.Show();
        }
    }

    public void ForceOpenHUD(GameState state)
    {
        if (_hudCache.TryGetValue(state, out var hud))
        {
            hud.Show();
        }
    }

    public void Show<T>() where T : WindowBase
    {
        HideAllWindows();

        var windowType = typeof(T);
        if (_windowCache.TryGetValue(windowType, out var window))
        {
            window.Show();
        }
    }

    public void HideAll()
    {
        HideAllWindows();
        HideAllHud();
    }

    private void HideAllWindows()
    {
        if (_config?.windows == null)
            return;

        foreach (var window in _config.windows)
        {
            if (window != null)
                window.Hide();
        }
    }

    private void HideAllHud()
    {
        foreach (var hud in _hudCache.Values)
        {
            if (hud != null)
                hud.Hide();
        }
    }
}
