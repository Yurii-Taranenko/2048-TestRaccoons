using UnityEngine;

public class InGameEvents : MonoBehaviour
{
}

// Initialization
public struct OnManagersInitialized { }

// Game State
public struct GameStateChangedEvent { public GameState GameState; }
public struct GameOverEvent { }
public struct OnRestartGameEvent { }

// Score
public struct ScoreChangedEvent { public int Score; }

// Input
public struct InputTouchStartedEvent { public Vector2 TouchPosition; }
public struct InputTouchMovedEvent { public Vector2 TouchPosition; }
public struct InputTouchEndedEvent { public Vector2 TouchPosition; }

// Cube move
public struct CubeDragEvent { public float DragDelta; }
public struct CubeLaunchEvent { public float Force; }
public struct CubeMergedEvent
{
    public int ResultingValue;
    public Vector3 MergePosition;
    public int ScoreGained;
}

// Collision
public struct CubeCollisionEvent
{
    public int ColliderCubeId;
    public int? CollidedWithCubeId;
    public float ImpactForce;
    public Vector3 CollisionPosition;
}
