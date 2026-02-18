using UnityEngine;

/// <summary>
/// Configuration for 2048 game mechanics.
/// Defines cube spawn probabilities and physics parameters.
/// </summary>
[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
public class GameplayConfig : ScriptableObject
{
    [Header("Cube Spawn")]
    [Range(0f, 1f)]
    [SerializeField] public float spawnProbability2 = 0.75f;
    [Range(0f, 1f)]
    [SerializeField] public float spawnProbability4 = 0.25f;

    [Header("Physics")]
    [SerializeField] public float minCollisionImpulse = 1f;
    [SerializeField] public float cubeSpawnForce = 10f;

    private void OnValidate()
    {
        float totalProb = spawnProbability2 + spawnProbability4;
        if (Mathf.Abs(totalProb - 1f) > 0.01f)
        {
            Debug.LogWarning($"[GameplayConfig] Sum of spawn probabilities is {totalProb}, should be 1.0");
        }
    }
}