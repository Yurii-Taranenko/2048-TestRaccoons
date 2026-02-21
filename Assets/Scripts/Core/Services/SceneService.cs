using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Core.State;

namespace Game.Core.Services
{
    public class SceneService : MonoBehaviour, ISceneService
    {
        [System.Serializable]
        public class SceneMapping
        {
            public GameState state;
            public string sceneName;
        }

        [SerializeField] private List<SceneMapping> _sceneMappings;

        private Dictionary<GameState, string> _sceneCache;

        [Inject]
        private void Construct()
        {
            _sceneCache = new Dictionary<GameState, string>();
            if (_sceneMappings != null)
            {
                foreach (var mapping in _sceneMappings)
                {
                    if (!string.IsNullOrEmpty(mapping.sceneName))
                        _sceneCache[mapping.state] = mapping.sceneName;
                }
            }

            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            if (_sceneCache.TryGetValue(evt.GameState, out var sceneName)
                && SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
