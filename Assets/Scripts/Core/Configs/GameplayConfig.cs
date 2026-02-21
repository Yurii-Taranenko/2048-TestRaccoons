using UnityEngine;

namespace Game.Core.Config
{
    /// <summary>
    /// Configuration for 2048 game mechanics.
    /// Defines cube spawn probabilities and physics parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [Header("Cube Spawn")]
        [Range(0f, 1f)]
        public float spawnProbability2 = 0.75f;
        [Range(0f, 1f)]
        public float spawnProbability4 = 0.25f;

        [Header("Physics")]
        public float minCollisionImpulse = 1f;
        public float cubeSpawnForce = 10f;
        public float collisionResetTime = 0.2f;

        [Header("Visuals")]
        public float mergeAnimationDuration = 0.3f;
        public bool enableMergeFeedback = true;

        [Header("Auto-Merge Booster")]
        public float autoMergeAnimationDuration = 1.5f;
        public float autoMergeRiseHeight = 3f;

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

            if (autoMergeAnimationDuration <= 0)
            {
                Debug.LogWarning("[GameplayConfig] autoMergeAnimationDuration must be > 0");
                autoMergeAnimationDuration = 1.5f;
            }
        }
    }
}