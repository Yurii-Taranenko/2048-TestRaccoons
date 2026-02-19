using UnityEngine;

/// <summary>
/// Initializes gameplay systems when the gameplay scene is loaded.
/// Connects managers from Bootstrap scene with gameplay-specific components.
/// 
/// This script should be placed on any GameObject in the gameplay scene.
/// It publishes GameplaySceneLoadedEvent so managers can receive runtime dependencies.
/// </summary>
public class GameplayInitializer : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CubeInputHandler inputHandler;

    private void Start()
    {
        if (spawnPoint == null || inputHandler == null)
        {
            Debug.LogError("[GameplayInitializer] Missing references (spawnPoint or inputHandler)!");
            return;
        }

        // Publish event that gameplay scene is loaded with its dependencies
        EventBus.Publish(new GameplaySceneLoadedEvent
        {
            SpawnPoint = spawnPoint,
            InputHandler = inputHandler
        });

        Debug.Log("[GameplayInitializer] Gameplay scene initialized and event published");
    }
}