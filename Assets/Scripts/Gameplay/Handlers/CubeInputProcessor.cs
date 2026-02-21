using UnityEngine;
using Zenject;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Core.Services;
using Game.Gameplay.Cubes;

namespace Game.Gameplay.Input
{
    public class CubeInputProcessor : IInitializable
    {
        private const float MAX_HORIZONTAL_POSITION = 2f;

        private readonly ICubeService _cubeService;

        private Cube _activeCube;
        private Vector3 _cubeStartPosition;
        private bool _isDragging;

        public CubeInputProcessor(ICubeService cubeService)
        {
            _cubeService = cubeService;
        }

        public void Initialize()
        {
            EventBus.Subscribe<InputTouchStartedEvent>(OnTouchStarted);
            EventBus.Subscribe<InputTouchMovedEvent>(OnTouchMoved);
            EventBus.Subscribe<InputTouchEndedEvent>(OnTouchEnded);
            EventBus.Subscribe<ActiveCubeChangedEvent>(OnActiveCubeChanged);
            EventBus.Subscribe<GameplayResetEvent>(OnReset);
        }

        private void OnActiveCubeChanged(ActiveCubeChangedEvent evt)
        {
            _activeCube = _cubeService.GetCubeById(evt.CubeId);
        }

        private void OnTouchStarted(InputTouchStartedEvent evt)
        {
            if (_activeCube == null) return;

            _cubeStartPosition = _activeCube.transform.position;
            _isDragging = true;
            _activeCube.Rigidbody.isKinematic = true;
        }

        private void OnTouchMoved(InputTouchMovedEvent evt)
        {
            if (!_isDragging || _activeCube == null) return;

            var cam = Camera.main;
            float distanceToCamera = Mathf.Abs(cam.transform.position.z - _cubeStartPosition.z);
            var screenPos = new Vector3(evt.TouchPosition.x, evt.TouchPosition.y, distanceToCamera);
            var worldPos = cam.ScreenToWorldPoint(screenPos);

            float clampedX = Mathf.Clamp(worldPos.x, -MAX_HORIZONTAL_POSITION, MAX_HORIZONTAL_POSITION);
            var newPos = _cubeStartPosition;
            newPos.x = clampedX;
            _activeCube.transform.position = newPos;
        }

        private void OnTouchEnded(InputTouchEndedEvent evt)
        {
            if (!_isDragging || _activeCube == null) return;

            _isDragging = false;
            _activeCube.Rigidbody.isKinematic = false;
            _activeCube.Launch();
            _activeCube = null;

            EventBus.Publish(new CubeLaunchEvent { Force = 1f });
        }

        private void OnReset(GameplayResetEvent evt)
        {
            _activeCube = null;
            _isDragging = false;
        }
    }
}
