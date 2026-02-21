using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Zenject;
using Game.Core.Events;
using Game.Core.Utils;

namespace Game.Core.Services
{
    public class InputService : MonoBehaviour, IInputService
    {
        private PlayerInput _playerInput;

        [Inject]
        private void Construct()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null) return;

            var click = _playerInput.actions.FindAction("Click");
            var touch = _playerInput.actions.FindAction("Touch");

            if (click != null)
            {
                click.started += OnClickStarted;
                click.canceled += OnClickCanceled;
            }

            if (touch != null)
                touch.performed += OnTouchMoved;
        }

        private void OnDestroy()
        {
            if (_playerInput == null) return;

            var click = _playerInput.actions.FindAction("Click");
            var touch = _playerInput.actions.FindAction("Touch");

            if (click != null)
            {
                click.started -= OnClickStarted;
                click.canceled -= OnClickCanceled;
            }

            if (touch != null)
                touch.performed -= OnTouchMoved;
        }

        private void OnClickStarted(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return;
            EventBus.Publish(new InputTouchStartedEvent { TouchPosition = GetTouchPosition() });
        }

        private void OnTouchMoved(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return;
            EventBus.Publish(new InputTouchMovedEvent { TouchPosition = GetTouchPosition() });
        }

        private void OnClickCanceled(InputAction.CallbackContext ctx)
        {
            EventBus.Publish(new InputTouchEndedEvent { TouchPosition = GetTouchPosition() });
        }

        private Vector2 GetTouchPosition()
        {
            if (Mouse.current != null) return Mouse.current.position.ReadValue();
            if (Touchscreen.current != null) return Touchscreen.current.position.ReadValue();
            return Vector2.zero;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
        }
    }
}
