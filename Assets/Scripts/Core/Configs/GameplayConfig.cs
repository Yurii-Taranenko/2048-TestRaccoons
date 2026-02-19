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
    [SerializeField] public float collisionResetTime = 0.2f;

    [Header("Visuals")]
    [SerializeField] public float mergeAnimationDuration = 0.3f;
    [SerializeField] public bool enableMergeFeedback = true;

    private void OnValidate()
    {
        float totalProb = spawnProbability2 + spawnProbability4;
        if (Mathf.Abs(totalProb - 1f) > 0.01f)
            Debug.LogWarning($"[GameplayConfig] Probabilities sum: {totalProb}, expected 1.0");

        if (minCollisionImpulse < 0)
        {
            Debug.LogWarning("[GameplayConfig] minCollisionImpulse must be >= 0");
            minCollisionImpulse = 0;
        }

        if (collisionResetTime <= 0)
        {
            Debug.LogWarning("[GameplayConfig] collisionResetTime must be > 0");
            collisionResetTime = 0.1f;
        }
    }
}