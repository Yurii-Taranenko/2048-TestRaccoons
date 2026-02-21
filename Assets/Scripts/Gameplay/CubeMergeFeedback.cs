using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Utils;

namespace Game.Gameplay.Merge
{
    public class CubeMergeFeedback : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mergePrefab;
        [SerializeField] private int _poolSize = 5;
        [SerializeField] private AudioSource _mergeSource;
        [SerializeField] private AudioSource _launchSound;

        private static readonly Vector3 ParticleOffset = Vector3.up;
        private readonly Queue<ParticleSystem> _particlePool = new();

        private void Awake()
        {
            if (_mergePrefab == null) return;

            for (int i = 0; i < _poolSize; i++)
            {
                var ps = Instantiate(_mergePrefab, transform);
                ps.gameObject.SetActive(false);
                _particlePool.Enqueue(ps);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<CubeLaunchEvent>(OnCubeLaunch);
            EventBus.Subscribe<CubeMergedEvent>(OnCubeMerged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CubeLaunchEvent>(OnCubeLaunch);
            EventBus.Unsubscribe<CubeMergedEvent>(OnCubeMerged);
        }

        private void OnCubeLaunch(CubeLaunchEvent evt)
        {
            PlayLaunchSound();
        }

        private void OnCubeMerged(CubeMergedEvent evt)
        {
            PlayParticleEffect(evt.MergePosition);
            PlayMergeSound();
        }

        private void PlayParticleEffect(Vector3 position)
        {
            if (_particlePool.Count == 0) return;

            var ps = _particlePool.Dequeue();
            ps.gameObject.SetActive(true);
            ps.transform.position = position + ParticleOffset;
            ps.Play();

            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            ReturnToPoolDelayed(ps, duration).Forget();
        }

        private async UniTask ReturnToPoolDelayed(ParticleSystem ps, float delay)
        {
            await UniTask.Delay((int)(delay * 1000), cancellationToken: destroyCancellationToken);
            if (ps == null) return;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.gameObject.SetActive(false);
            _particlePool.Enqueue(ps);
        }

        private void PlayMergeSound()
        {
            if (_mergeSource != null && _mergeSource.clip != null)
                _mergeSource.PlayOneShot(_mergeSource.clip);
        }

        private void PlayLaunchSound()
        {
            if (_launchSound != null && _launchSound.clip != null)
                _launchSound.PlayOneShot(_launchSound.clip);
        }
    }
}