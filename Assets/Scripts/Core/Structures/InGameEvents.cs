using UnityEngine;

public class InGameEvents : MonoBehaviour
{
}

public struct OnManagersInitialized { }
public struct GameplaySceneLoadedEvent 
{ 
    public Transform SpawnPoint;
    public CubeInputHandler InputHandler;
}

public struct GameStateChangedEvent { public GameState GameState; }
public struct GameOverEvent { }
public struct OnRestartGameEvent { }

public struct ScoreChangedEvent { public int Score; }

public struct InputTouchStartedEvent { public Vector2 TouchPosition; }
public struct InputTouchMovedEvent { public Vector2 TouchPosition; }
public struct InputTouchEndedEvent { public Vector2 TouchPosition; }

public struct CubeDragEvent { public float DragDelta; }
public struct CubeLaunchEvent { public float Force; }
public struct CubeMergedEvent
{
    public int ResultingValue;
    public Vector3 MergePosition;
    public int ScoreGained;
}

public struct CubeCollisionEvent
{
    public int ColliderCubeId;
    public int? CollidedWithCubeId;
    public float ImpactForce;
    public Vector3 CollisionPosition;
}

public struct CubeCollisionRequestEvent
{
    public Collision Collision;
    public int InitiatorCubeId;
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
