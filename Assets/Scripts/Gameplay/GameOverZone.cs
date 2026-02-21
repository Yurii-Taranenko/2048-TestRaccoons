using UnityEngine;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Gameplay.Cubes;

namespace Game.Gameplay
{
    /// <summary>
    /// Detects when cubes fall into the game over zone.
    /// Publishes GameOverEvent when a cube collides with the trigger.
    /// </summary>
    public class GameOverZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Cube")) return;

            var cube = other.GetComponentInParent<Cube>();
            if (cube != null)
                EventBus.Publish(new GameOverEvent { CubeId = cube.Id });
        }
    }
}
