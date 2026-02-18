using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private List<BaseManager> _managers;
    [SerializeField] private GamePresenter _gamePresenter;

    private void Awake()
    {
        //For production, we can use a DI-container framework or service locator pattern. But for this simple project, i will just manually inject dependencies (but only once on start).

        _gamePresenter.Init(_managers);
    }
}
