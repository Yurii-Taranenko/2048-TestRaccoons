using UnityEngine;

/// <summary>
/// Main game config
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] public WindowConfig windowConfig;
    [SerializeField] public GameplayConfig gameplayConfig;

    private static GameConfig _instance;

    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ConfigManager.LoadConfig<GameConfig>("GameConfig");
                if (_instance == null)
                {
                    Debug.LogError("[GameConfig] GameConfig not found in Assets/Resources/Configs/");
                }
            }
            return _instance;
        }
    }

    public void LoadAllConfigs()
    {
        if (windowConfig == null)
            windowConfig = ConfigManager.LoadConfig<WindowConfig>("WindowConfig");

        if (gameplayConfig == null)
            gameplayConfig = ConfigManager.LoadConfig<GameplayConfig>("GameplayConfig");

        if (windowConfig == null || gameplayConfig == null)
            Debug.LogError("[GameConfig] Failed to load required configs!");
    }
}