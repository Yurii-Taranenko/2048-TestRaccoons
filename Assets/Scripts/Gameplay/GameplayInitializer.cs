using UnityEngine;
using Game.Core.Events;
using Game.Core.Utils;

namespace Game.Gameplay
{
    public class GameplayInitializer : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;

        private void Start()
        {
            if (spawnPoint == null)
            {
                Debug.LogError("[GameplayInitializer] Missing spawn point!");
                return;
            }

            EventBus.Publish(new GameplaySceneLoadedEvent
            {
                SpawnPoint = spawnPoint
            });
        }
    }
}