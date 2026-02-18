using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerCustom : BaseManager
{
    [System.Serializable]
    public class SceneMapping
    {
        public GameState state;
        public string sceneName;
    }

    [SerializeField] private List<SceneMapping> sceneMappings;

    public override void Init()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
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

    private string GetSceneNameForState(GameState state)
    {
        foreach (var mapping in sceneMappings)
        {
            if (mapping.state == state)
                return mapping.sceneName;
        }
        return null;
    }

    internal void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(activeScene.name);


    }
}
