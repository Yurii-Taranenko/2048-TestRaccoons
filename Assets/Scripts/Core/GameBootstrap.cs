using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private List<BaseManager> _managers;
    [SerializeField] private GamePresenter _gamePresenter;

    [Header("Configs")]
    [SerializeField] private WindowConfig _windowConfig;
    [SerializeField] private GameplayConfig _gameplayConfig;

    private void Awake()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        LoadConfigs();
        InjectConfigs();

        GameModel gameModel = new GameModel();
        _gamePresenter.Init(_managers, gameModel);

        Debug.Log("[GameBootstrap] Game ready");
    }

    private bool ValidateSetup()
    {
        bool isValid = true;

        if (_managers == null || _managers.Count == 0)
        {
            Debug.LogError("[GameBootstrap] Managers list empty");
            isValid = false;
        }

        if (_gamePresenter == null)
        {
            Debug.LogError("[GameBootstrap] GamePresenter not assigned");
            isValid = false;
        }

        return isValid;
    }

    private void LoadConfigs()
    {
        if (_windowConfig == null)
        {
            _windowConfig = ConfigManager.LoadConfig<WindowConfig>("WindowConfig");
            if (_windowConfig == null)
                Debug.LogError("[GameBootstrap] WindowConfig not found");
        }

        if (_gameplayConfig == null)
        {
            _gameplayConfig = ConfigManager.LoadConfig<GameplayConfig>("GameplayConfig");
            if (_gameplayConfig == null)
                Debug.LogError("[GameBootstrap] GameplayConfig not found");
        }
    }

    private void InjectConfigs()
    {
        foreach (var manager in _managers)
        {
            if (manager is WindowManager windowManager && _windowConfig != null)
                windowManager.SetConfig(_windowConfig);
            else if (manager is GameplayManager gameplayManager && _gameplayConfig != null)
                gameplayManager.SetConfig(_gameplayConfig);
            else if (manager is CubeManager cubeManager && _gameplayConfig != null)
            {
                cubeManager.SetConfig(_gameplayConfig);
                
                var colorConfig = ConfigManager.LoadConfig<CubeColorConfig>("CubeColorConfig");
                if (colorConfig != null)
                    cubeManager.SetColorConfig(colorConfig);
            }
        }
    }
}
