using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Game.Core.Config;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Gameplay.Cubes;
using UnityCollision = UnityEngine.Collision;

namespace Game.Gameplay.Collision
{
    public class CubeCollisionHandler : IInitializable, ITickable
    {
        private static readonly Vector3 MergeOffset = new(0f, 1f, 0f);

        private readonly GameplayConfig _config;

        private HashSet<(int, int)> _processedCollisions = new();
        private float _collisionResetTimer;
        private Dictionary<int, Cube> _cubesById;
        private ObjectPool<Cube> _cubePool;

        public CubeCollisionHandler(GameplayConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            EventBus.Subscribe<CubeCollisionRequestEvent>(OnCubeCollisionRequest);
            EventBus.Subscribe<GameplayResetEvent>(OnReset);
        }

        public void SetRuntimeDependencies(Dictionary<int, Cube> cubesById, ObjectPool<Cube> cubePool)
        {
            _cubesById = cubesById;
            _cubePool = cubePool;
        }

        public void Tick()
        {
            if (_config == null) return;

            _collisionResetTimer += Time.deltaTime;
            if (_collisionResetTimer >= _config.collisionResetTime)
            {
                _processedCollisions.Clear();
                _collisionResetTimer = 0f;
            }
        }

        private void OnCubeCollisionRequest(CubeCollisionRequestEvent evt)
        {
            if (_cubesById == null) return;

            _cubesById.TryGetValue(evt.InitiatorCubeId, out var initiator);
            if (initiator == null) return;

            var other = evt.Collision.gameObject.GetComponent<Cube>();
            if (other == null || !initiator.IsAlive || !other.IsAlive) return;

            ProcessCollision(initiator, other, evt.Collision);
        }

        private void ProcessCollision(Cube cube1, Cube cube2, UnityCollision collision)
        {
            int id1 = cube1.Id;
            int id2 = cube2.Id;
            if (id1 > id2) (id1, id2) = (id2, id1);

            if (!_processedCollisions.Add((id1, id2))) return;
            if (collision.relativeVelocity.magnitude < _config.minCollisionImpulse) return;
            if (cube1.Value != cube2.Value) return;

            ExecuteMerge(cube1, cube2, collision.contacts[0].point);
        }

        private void ExecuteMerge(Cube cube1, Cube cube2, Vector3 mergePosition)
        {
            int resultValue = cube1.Value * 2;

            cube1.MergeWith(resultValue);
            cube2.MergeWith(resultValue);
            cube1.ApplyPostMergePhysics(mergePosition + MergeOffset);

            _cubesById.Remove(cube2.Id);
            cube2.gameObject.SetActive(false);
            _cubePool.Return(cube2);

            EventBus.Publish(new CubeMergedEvent
            {
                ResultingValue = resultValue,
                MergePosition = mergePosition,
                ScoreGained = resultValue / 2
            });
        }

        private void OnReset(GameplayResetEvent evt)
        {
            _processedCollisions.Clear();
            _collisionResetTimer = 0f;
        }
    }
}
