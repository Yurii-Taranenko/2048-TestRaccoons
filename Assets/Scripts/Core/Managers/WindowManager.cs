using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : BaseManager
{
    private WindowConfig _config;
    private Dictionary<Type, WindowBase> _windowCache;
    private Dictionary<GameState, WindowBase> _hudCache;
    private Transform _windowsParent;
    private Transform _hudsParent;

    [SerializeField] private Transform windowsParent;
    [SerializeField] private Transform hudsParent;

    public void SetConfig(WindowConfig config)
    {
        _config = config;
    }

    public override void Init()
    {
        if (_config == null)
        {
            Debug.LogError("[WindowManager] Config not assigned");
            return;
        }

        _windowsParent = windowsParent;
        _hudsParent = hudsParent;

        if (_windowsParent == null || _hudsParent == null)
        {
            Debug.LogError("[WindowManager] Parents not assigned in Inspector");
            return;
        }

        SpawnWindowsFromPrefabs();
        SpawnHudsFromPrefabs();
        EventBus.Subscribe<GameStateChangedEvent>(UpdateHUD);

        Debug.Log($"[WindowManager] Ready: {_windowCache.Count} windows, {_hudCache.Count} HUDs");
    }

    private void SpawnWindowsFromPrefabs()
    {
        _windowCache = new Dictionary<Type, WindowBase>();

        if (_config?.windows == null || _config.windows.Count == 0)
        {
            Debug.LogWarning("[WindowManager] Windows list empty");
            return;
        }

        foreach (var windowPrefab in _config.windows)
        {
            if (windowPrefab == null)
                continue;

            try
            {
                WindowBase spawnedWindow = Instantiate(windowPrefab, _windowsParent);
                spawnedWindow.gameObject.name = windowPrefab.GetType().Name;
                spawnedWindow.gameObject.SetActive(false);

                var windowType = windowPrefab.GetType();
                _windowCache[windowType] = spawnedWindow;

                Debug.Log($"[WindowManager] Window spawned: {windowType.Name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WindowManager] Spawn failed: {ex.Message}");
            }
        }
    }

    private void SpawnHudsFromPrefabs()
    {
        _hudCache = new Dictionary<GameState, WindowBase>();

        if (_config?.hudMappings == null || _config.hudMappings.Count == 0)
        {
            Debug.LogWarning("[WindowManager] HUD mappings empty");
            return;
        }

        foreach (var mapping in _config.hudMappings)
        {
            if (mapping?.hud == null)
                continue;

            try
            {
                WindowBase spawnedHud = Instantiate(mapping.hud, _hudsParent);
                spawnedHud.gameObject.name = $"HUD_{mapping.state}";
                spawnedHud.gameObject.SetActive(false);

                _hudCache[mapping.state] = spawnedHud;

                Debug.Log($"[WindowManager] HUD spawned: {mapping.state}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WindowManager] HUD spawn failed: {ex.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(UpdateHUD);
    }

    public void UpdateHUD(GameStateChangedEvent evt)
    {
        Debug.Log($"[WindowManager] Update HUD: {evt.GameState}");
        HideAllHud();

        if (_hudCache.TryGetValue(evt.GameState, out var hud))
        {
            hud.Show();
            Debug.Log($"[WindowManager] HUD shown: {hud.gameObject.name}");
        }
    }

    public void ForceOpenHUD(GameState state)
    {
        if (_hudCache.TryGetValue(state, out var hud))
        {
            HideAllHud();
            hud.Show();
        }
    }

    public void Show<T>() where T : WindowBase
    {
        Debug.Log($"[WindowManager] Show: {typeof(T).Name}");
        HideAllWindows();

        var windowType = typeof(T);
        if (_windowCache.TryGetValue(windowType, out var window))
        {
            window.Show();
            Debug.Log($"[WindowManager] Window shown: {window.gameObject.name}");
        }
        else
        {
            Debug.LogError($"[WindowManager] Window not found: {windowType.Name}");
        }
    }

    public void HideAll()
    {
        HideAllWindows();
        HideAllHud();
    }

    private void HideAllWindows()
    {
        foreach (var window in _windowCache.Values)
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
