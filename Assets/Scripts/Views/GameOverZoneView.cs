using UnityEngine;

public class GameOverZoneView : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
            EventBus.Publish(new GameOverEvent());
    }
}
