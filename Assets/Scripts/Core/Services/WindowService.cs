using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Game.Core.Config;
using Game.Core.Events;
using Game.Core.State;
using Game.Core.Utils;
using Game.UI.Base;

namespace Game.Core.Services
{
    public class WindowService : MonoBehaviour, IWindowService
    {
        [SerializeField] private Transform _windowsParent;
        [SerializeField] private Transform _hudsParent;

        private DiContainer _container;
        private Dictionary<Type, WindowBase> _windowCache;
        private Dictionary<GameState, WindowBase> _hudCache;

        [Inject]
        private void Construct(DiContainer container, WindowConfig config)
        {
            _container = container;
            _windowCache = new Dictionary<Type, WindowBase>();
            _hudCache = new Dictionary<GameState, WindowBase>();

            SpawnWindows(config);
            SpawnHuds(config);

            EventBus.Subscribe<GameStateChangedEvent>(OnStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnStateChanged);
        }

        private void SpawnWindows(WindowConfig config)
        {
            if (config?.windows == null) return;

            foreach (var prefab in config.windows)
            {
                if (prefab == null) continue;
                var instance = _container.InstantiatePrefabForComponent<WindowBase>(prefab, _windowsParent);
                instance.gameObject.SetActive(false);
                _windowCache[prefab.GetType()] = instance;
            }
        }

        private void SpawnHuds(WindowConfig config)
        {
            if (config?.hudMappings == null) return;

            foreach (var mapping in config.hudMappings)
            {
                if (mapping?.hud == null) continue;
                var instance = _container.InstantiatePrefabForComponent<WindowBase>(mapping.hud, _hudsParent);
                instance.gameObject.SetActive(false);
                _hudCache[mapping.state] = instance;
            }
        }

        private void OnStateChanged(GameStateChangedEvent evt)
        {
            HideAllHuds();
            if (_hudCache.TryGetValue(evt.GameState, out var hud))
                hud.Show();
        }

        public void Show<T>() where T : WindowBase
        {
            HideAllWindows();
            if (_windowCache.TryGetValue(typeof(T), out var window))
                window.Show();
        }

        public void ForceOpenHUD(GameState state)
        {
            HideAllHuds();
            if (_hudCache.TryGetValue(state, out var hud))
                hud.Show();
        }

        public void HideAllWindows()
        {
            foreach (var w in _windowCache.Values)
                w?.Hide();
        }

        public void HideAllHuds()
        {
            foreach (var h in _hudCache.Values)
                h?.Hide();
        }

        public T GetHud<T>() where T : WindowBase
        {
            foreach (var hud in _hudCache.Values)
            {
                if (hud is T typed)
                    return typed;
            }
            return null;
        }
    }
}
