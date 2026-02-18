using UnityEngine;

public class InGameEvents : MonoBehaviour
{}

public struct OnManagersInitialized {}
public struct OnRestartGameEvent { }
public struct GameOverEvent { }
public struct ScoreChangedEvent { public int Score; }
public struct GameStateChangedEvent { public GameState GameState; }
