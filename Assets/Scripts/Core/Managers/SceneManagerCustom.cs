using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene loading based on game state changes.
/// Caches state-to-scene mappings for optimal performance.
/// </summary>
public class SceneManagerCustom : BaseManager
{
    [System.Serializable]
    public class SceneMapping
    {
        public GameState state;
        public string sceneName;
    }

    [SerializeField] private List<SceneMapping> sceneMappings;
    private Dictionary<GameState, string> _sceneCache;

    public override void Init()
    {
        InitializeSceneCache();
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void InitializeSceneCache()
    {
        _sceneCache = new Dictionary<GameState, string>();
        if (sceneMappings != null)
        {
            foreach (var mapping in sceneMappings)
            {
                if (!string.IsNullOrEmpty(mapping.sceneName))
                    _sceneCache[mapping.state] = mapping.sceneName;
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        string sceneName = GetSceneNameForState(evt.GameState);
        if (!string.IsNullOrEmpty(sceneName) && SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Retrieves scene name for a given game state using cached dictionary.
    /// O(1) lookup instead of O(n) iteration.
    /// </summary>
    private string GetSceneNameForState(GameState state)
    {
        if (_sceneCache.TryGetValue(state, out var sceneName))
            return sceneName;

        Debug.LogWarning($"[SceneManagerCustom] No scene mapping found for state: {state}");
        return null;
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    internal void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
