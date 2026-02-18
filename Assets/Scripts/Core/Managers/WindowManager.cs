using System;
using System.Collections.Generic;
using UnityEngine;
using static GameStateManager;

public class WindowManager : BaseManager
{
    [SerializeField] private List<WindowBase> windows;

    [SerializeField] private List<HUDMapping> hudMappings;

    public override void Init()
    {
        EventBus.Subscribe<GameStateChangedEvent>(UpdateHUD);
    }

    public void UpdateHUD(GameStateChangedEvent evt)
    {
        foreach (var mapping in hudMappings)
        {
            if (mapping.hud != null)
            {
                if (mapping.state == evt.GameState)
                {
                    mapping.hud.Show();
                }
            }
        }
    }

    public void ForceOpenHUD(GameState state)
    {
        foreach (var mapping in hudMappings)
        {
            if (mapping.hud != null)
            {
                if (state == mapping.state)
                {
                    mapping.hud.Show();
                }
            }
        }
    }

    public void Show<T>() where T : WindowBase
    {
        foreach (var window in windows)
            window.Hide();

        var win = windows.Find(w => w is T);
        if (win != null)
            win.Show();
    }

    public void HideAll()
    {
        foreach (var window in windows)
            window.Hide();
    }
}
