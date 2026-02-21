using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core.Config;
using Game.Core.Events;
using Game.Core.Services;
using Game.Core.Utils;
using Game.Gameplay.Cubes;

namespace Game.Gameplay.Boosters
{
    public class AutoMergeBooster : IInitializable
    {
        private readonly ICubeService _cubeService;
        private readonly GameplayConfig _config;

        private CancellationTokenSource _cts;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public AutoMergeBooster(ICubeService cubeService, GameplayConfig config)
        {
            _cubeService = cubeService;
            _config = config;
        }

        public void Initialize()
        {
            EventBus.Subscribe<GameplayResetEvent>(OnReset);
        }

        // Async entry point — caller awaits until the full merge animation completes
        public async UniTask<bool> RunAsync()
        {
            if (_isRunning) return false;

            var pair = FindMergePair();
            if (pair == null) return false;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            return await ExecuteAutoMerge(pair.Value.cube1, pair.Value.cube2, _cts.Token);
        }

        private async UniTask<bool> ExecuteAutoMerge(Cube cube1, Cube cube2, CancellationToken ct)
        {
            _isRunning = true;

            try
            {
                cube1.Rigidbody.isKinematic = true;
                cube2.Rigidbody.isKinematic = true;

                float duration = _config.autoMergeAnimationDuration;
                float riseHeight = _config.autoMergeRiseHeight;

                Vector3 start1 = cube1.transform.position;
                Vector3 start2 = cube2.transform.position;

                // Rise above the field
                Vector3 risen1 = start1 + Vector3.up * riseHeight;
                Vector3 risen2 = start2 + Vector3.up * riseHeight;
                await AnimateMove(cube1.transform, cube2.transform,
                    start1, start2, risen1, risen2, duration * 0.3f, ct);

                // Swing back
                Vector3 swingDir = (risen1 - risen2).normalized;
                Vector3 swing1 = risen1 + swingDir * 0.5f;
                Vector3 swing2 = risen2 - swingDir * 0.5f;
                await AnimateMove(cube1.transform, cube2.transform,
                    risen1, risen2, swing1, swing2, duration * 0.2f, ct);

                // Fly into each other
                Vector3 mergePoint = (swing1 + swing2) * 0.5f;
                await AnimateMove(cube1.transform, cube2.transform,
                    swing1, swing2, mergePoint, mergePoint, duration * 0.5f, ct);

                // Apply merge
                int resultValue = cube1.Value * 2;
                cube1.MergeWith(resultValue);
                cube1.ApplyPostMergePhysics(mergePoint);
                _cubeService.RemoveCube(cube2.Id);

                EventBus.Publish(new CubeMergedEvent
                {
                    ResultingValue = resultValue,
                    MergePosition = mergePoint,
                    ScoreGained = resultValue / 2
                });

                return true;
            }
            catch (System.OperationCanceledException)
            {
                return false;
            }
            finally
            {
                _isRunning = false;
            }
        }

        private async UniTask AnimateMove(
            Transform t1, Transform t2,
            Vector3 from1, Vector3 from2,
            Vector3 to1, Vector3 to2,
            float duration, CancellationToken ct)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                ct.ThrowIfCancellationRequested();
                if (t1 == null || t2 == null) return;

                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                t1.position = Vector3.Lerp(from1, to1, t);
                t2.position = Vector3.Lerp(from2, to2, t);

                await UniTask.Yield(ct);
            }

            if (t1 != null) t1.position = to1;
            if (t2 != null) t2.position = to2;
        }

        private (Cube cube1, Cube cube2)? FindMergePair()
        {
            var cubes = _cubeService.GetActiveCubes();
            var activeCube = _cubeService.ActiveCube;

            // Find first group of 2+ cubes with same value, excluding the active (unlaunched) cube
            var candidates = cubes.Values
                .Where(c => c.IsAlive && c.gameObject.activeSelf
                    && (activeCube == null || c.Id != activeCube.Id))
                .GroupBy(c => c.Value)
                .FirstOrDefault(g => g.Count() >= 2);

            if (candidates == null) return null;

            var list = candidates.Take(2).ToArray();
            return (list[0], list[1]);
        }

        private void OnReset(GameplayResetEvent evt)
        {
            _cts?.Cancel();
            _isRunning = false;
        }
    }
}
