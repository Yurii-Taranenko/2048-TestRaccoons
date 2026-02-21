using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core.Config;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Gameplay.Cubes;
using Game.Gameplay.Collision;

namespace Game.Core.Services
{
    public class CubeService : MonoBehaviour, ICubeService
    {
        private const int INITIAL_POOL_SIZE = 10;
        private const float SPAWN_DELAY_MS = 500;

        [SerializeField] private Cube _cubePrefab;

        private GameplayConfig _gameplayConfig;
        private CubeColorConfig _colorConfig;
        private CubeCollisionHandler _collisionHandler;

        private ObjectPool<Cube> _cubePool;
        private Cube _activeCube;
        private int _cubeIdCounter;
        private Dictionary<int, Cube> _cubesById = new();
        private CancellationTokenSource _spawnCts;
        private Transform _spawnPoint;
        private bool _isGameplayReady;

        public Cube ActiveCube => _activeCube;

        [Inject]
        private void Construct(
            GameplayConfig gameplayConfig,
            CubeColorConfig colorConfig,
            CubeCollisionHandler collisionHandler)
        {
            _gameplayConfig = gameplayConfig;
            _colorConfig = colorConfig;
            _collisionHandler = collisionHandler;

            EventBus.Subscribe<CubeLaunchEvent>(OnCubeLaunched);
            EventBus.Subscribe<GameplaySceneLoadedEvent>(OnGameplaySceneLoaded);
        }

        private void OnDestroy()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            EventBus.Unsubscribe<CubeLaunchEvent>(OnCubeLaunched);
            EventBus.Unsubscribe<GameplaySceneLoadedEvent>(OnGameplaySceneLoaded);
        }

        public void InitializeGameplay(Transform spawnPoint)
        {
            if (_isGameplayReady || spawnPoint == null) return;

            _spawnPoint = spawnPoint;
            _cubePool = new ObjectPool<Cube>(_cubePrefab, INITIAL_POOL_SIZE, _spawnPoint);
            _collisionHandler.SetRuntimeDependencies(_cubesById, _cubePool);

            SpawnNewCube();
            _isGameplayReady = true;
        }

        public void ResetCubes()
        {
            _spawnCts?.Cancel();

            foreach (var cube in _cubesById.Values)
            {
                cube.gameObject.SetActive(false);
                _cubePool.Return(cube);
            }

            _cubePool.Reset();
            _cubesById.Clear();
            _cubeIdCounter = 0;
            _activeCube = null;

            EventBus.Publish(new GameplayResetEvent());
        }

        public Cube GetCubeById(int id)
        {
            _cubesById.TryGetValue(id, out var cube);
            return cube;
        }

        public Dictionary<int, Cube> GetActiveCubes() => new(_cubesById);

        public void RemoveCube(int id)
        {
            if (!_cubesById.TryGetValue(id, out var cube)) return;

            _cubesById.Remove(id);
            cube.gameObject.SetActive(false);
            cube.transform.rotation = Quaternion.identity;
            _cubePool.Return(cube);
        }

        private void SpawnNewCube()
        {
            if (_spawnPoint == null) return;

            _activeCube = _cubePool.Get();
            _activeCube.transform.position = _spawnPoint.position;
            _activeCube.transform.rotation = Quaternion.identity;
            _activeCube.transform.SetParent(_spawnPoint);
            _activeCube.SetColorConfig(_colorConfig);
            _activeCube.Initialize(_gameplayConfig, _cubeIdCounter++);

            if (!_cubesById.ContainsKey(_activeCube.Id))
                _cubesById.Add(_activeCube.Id, _activeCube);

            EventBus.Publish(new ActiveCubeChangedEvent { CubeId = _activeCube.Id });
            EventBus.Publish(new CubeSpawnedEvent { CubeId = _activeCube.Id });
        }

        private void OnCubeLaunched(CubeLaunchEvent evt)
        {
            if (!_isGameplayReady) return;

            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            _spawnCts = new CancellationTokenSource();
            SpawnNewCubeDelayedAsync(_spawnCts.Token).Forget();
        }

        private async UniTaskVoid SpawnNewCubeDelayedAsync(CancellationToken ct)
        {
            await UniTask.Delay((int)SPAWN_DELAY_MS, cancellationToken: ct);
            if (_spawnPoint != null)
                SpawnNewCube();
        }

        private void OnGameplaySceneLoaded(GameplaySceneLoadedEvent evt)
        {
            _isGameplayReady = false;
            InitializeGameplay(evt.SpawnPoint);
        }
    }
}
