using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : BaseManager
{
    [SerializeField] private Cube cubePrefab;

    private ObjectPool<Cube> _cubePool;
    private Cube _activeCube;
    private GameplayConfig _config;
    private int _cubeIdCounter;

    private Dictionary<int, Cube> _cubesById = new Dictionary<int, Cube>();
    private HashSet<(int, int)> _processedCollisions = new HashSet<(int, int)>();
    private float _collisionResetTimer;
    private Vector3 mergeOffset = new Vector3(0f, 1f, 0f);
    private Coroutine _spawnCoroutine;

    private Transform _spawnPoint;
    private bool _isInitialized;
    private CubeColorConfig _colorConfig;

    public GameplayConfig Config => _config;
    public Cube ActiveCube => _activeCube;

    public void SetConfig(GameplayConfig config)
    {
        _config = config;
    }

    public void InitializeGameplay(Transform spawnPoint, CubeInputHandler inputHandler)
    {
        if (_isInitialized)
        {
            Debug.LogWarning("[CubeManager] Already initialized!");
            return;
        }

        if (spawnPoint == null || inputHandler == null)
        {
            Debug.LogError("[CubeManager] Missing dependencies");
            return;
        }

        _spawnPoint = spawnPoint;
        _cubePool = new ObjectPool<Cube>(cubePrefab, 10, transform);
        _collisionResetTimer = 0f;
        
        inputHandler.Initialize(this);
        
        SpawnNewCube();
        _isInitialized = true;

        Debug.Log("[CubeManager] Gameplay ready");
    }

    public override void Init()
    {
        if (_config == null)
        {
            Debug.LogError("[CubeManager] Config not assigned");
            return;
        }

        if (cubePrefab == null)
        {
            Debug.LogError("[CubeManager] Cube prefab missing");
            return;
        }

        EventBus.Subscribe<CubeLaunchEvent>(OnCubeLaunched);
        EventBus.Subscribe<GameplaySceneLoadedEvent>(OnGameplaySceneLoaded);
        EventBus.Subscribe<CubeCollisionRequestEvent>(OnCubeCollisionRequest);
        EventBus.Subscribe<ActiveCubeChangedEvent>(OnActiveCubeChangedRequest);

        Debug.Log("[CubeManager] Bootstrap done");
    }

    private void Update()
    {
        if (!_isInitialized)
            return;
        UpdateCollisionTracking();
    }

    private void UpdateCollisionTracking()
    {
        _collisionResetTimer += Time.deltaTime;
        if (_collisionResetTimer >= _config.collisionResetTime)
        {
            _processedCollisions.Clear();
            _collisionResetTimer = 0f;
        }
    }

    private void SpawnNewCube()
    {
        _activeCube = _cubePool.Get();
        _activeCube.transform.position = _spawnPoint.position;
        
        if (_colorConfig != null)
            _activeCube.SetColorConfig(_colorConfig);

        _activeCube.Initialize(_config, _cubeIdCounter++);
        RegisterCube(_activeCube);
        
        EventBus.Publish(new ActiveCubeChangedEvent { CubeId = _activeCube.Id });
        EventBus.Publish(new CubeSpawnedEvent { CubeId = _activeCube.Id });

        Debug.Log($"[CubeManager] Cube #{_activeCube.Id} spawned");
    }

    private void RegisterCube(Cube cube)
    {
        if (!_cubesById.ContainsKey(cube.Id))
            _cubesById.Add(cube.Id, cube);
    }

    private void UnregisterCube(Cube cube)
    {
        _cubesById.Remove(cube.Id);
    }

    private void OnActiveCubeChangedRequest(ActiveCubeChangedEvent evt)
    {
        EventBus.Publish(new ActiveCubeRequestEvent { CubeId = evt.CubeId });
    }

    private void OnCubeCollisionRequest(CubeCollisionRequestEvent evt)
    {
        if (!_isInitialized)
            return;

        var initiatorCube = GetCubeById(evt.InitiatorCubeId);
        if (initiatorCube == null)
            return;

        var otherCube = evt.Collision.gameObject.GetComponent<Cube>();
        if (otherCube == null)
            return;

        if (!initiatorCube.IsAlive || !otherCube.IsAlive)
            return;

        ProcessCollision(initiatorCube, otherCube, evt.Collision);
    }

    private Cube GetCubeById(int id)
    {
        _cubesById.TryGetValue(id, out var cube);
        return cube;
    }

    private void ProcessCollision(Cube cube1, Cube cube2, Collision collision)
    {
        int id1 = cube1.Id;
        int id2 = cube2.Id;

        if (id1 > id2)
            (id1, id2) = (id2, id1);

        if (_processedCollisions.Contains((id1, id2)))
            return;

        _processedCollisions.Add((id1, id2));

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce < _config.minCollisionImpulse)
            return;

        if (cube1.Value != cube2.Value)
            return;

        ExecuteMerge(cube1, cube2, collision.contacts[0].point);
    }

    private void ExecuteMerge(Cube cube1, Cube cube2, Vector3 mergePosition)
    {
        int resultValue = cube1.Value * 2;
        int scoreGained = resultValue / 2;

        cube1.MergeWith(resultValue);
        cube2.MergeWith(resultValue);
        cube1.ApplyPostMergePhysics(mergePosition + mergeOffset);

        UnregisterCube(cube2);
        cube2.gameObject.SetActive(false);
        _cubePool.Return(cube2);

        Debug.Log($"[CubeManager] Merged: {cube1.Value / 2}+{cube2.Value / 2}={resultValue}");

        EventBus.Publish(new CubeMergedEvent
        {
            ResultingValue = resultValue,
            MergePosition = mergePosition,
            ScoreGained = scoreGained
        });
    }

    private void OnCubeLaunched(CubeLaunchEvent evt)
    {
        if (!_isInitialized)
            return;

        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(SpawnNewCubeDelayed(0.5f));
    }

    private IEnumerator SpawnNewCubeDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewCube();
    }

    private void OnGameplaySceneLoaded(GameplaySceneLoadedEvent evt)
    {
        InitializeGameplay(evt.SpawnPoint, evt.InputHandler);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<CubeLaunchEvent>(OnCubeLaunched);
        EventBus.Unsubscribe<GameplaySceneLoadedEvent>(OnGameplaySceneLoaded);
        EventBus.Unsubscribe<CubeCollisionRequestEvent>(OnCubeCollisionRequest);
        EventBus.Unsubscribe<ActiveCubeChangedEvent>(OnActiveCubeChangedRequest);
    }

    public Dictionary<int, Cube> GetActiveCubes() => new Dictionary<int, Cube>(_cubesById);

    public void SetColorConfig(CubeColorConfig colorConfig)
    {
        _colorConfig = colorConfig;
    }

    public Cube GetCubeByIdPublic(int id) => GetCubeById(id);
}
