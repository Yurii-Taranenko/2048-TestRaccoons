using UnityEngine;
using Game.Core.State;

namespace Game.Core.Events
{
    public struct OnManagersInitialized { }

    public struct GameplaySceneLoadedEvent
    {
        public Transform SpawnPoint;
    }

    public struct GameStateChangedEvent
    {
        public GameState GameState;
    }

    public struct GameOverEvent
    {
        public int CubeId;
    }

    public struct OnRestartGameEvent { }
    public struct GameplayResetEvent { }

    public struct ScoreChangedEvent
    {
        public int Score;
    }

    public struct InputTouchStartedEvent
    {
        public Vector2 TouchPosition;
    }

    public struct InputTouchMovedEvent
    {
        public Vector2 TouchPosition;
    }

    public struct InputTouchEndedEvent
    {
        public Vector2 TouchPosition;
    }

    public struct CubeLaunchEvent
    {
        public float Force;
    }

    public struct CubeSpawnedEvent
    {
        public int CubeId;
    }

    public struct ActiveCubeChangedEvent
    {
        public int CubeId;
    }

    public struct ActiveCubeRequestEvent
    {
        public int CubeId;
    }

    public struct CubeCollisionRequestEvent
    {
        public Collision Collision;
        public int InitiatorCubeId;
    }

    public struct CubeCollisionEvent
    {
        public int ColliderCubeId;
        public int? CollidedWithCubeId;
        public float ImpactForce;
        public Vector3 CollisionPosition;
    }

    public struct CubeMergedEvent
    {
        public int ResultingValue;
        public Vector3 MergePosition;
        public int ScoreGained;
    }

    public struct AutoMergeStartedEvent
    {
        public int CubeId1;
        public int CubeId2;
    }

    public struct AutoMergeCompletedEvent
    {
        public int ResultCubeId;
        public int ResultValue;
    }
}
