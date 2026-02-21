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
        private bool _isPressed;

        [Inject]
        private void Construct()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null) return;

            var click = _playerInput.actions.FindAction("Click");

            if (click != null)
            {
                click.started += OnClickStarted;
                click.canceled += OnClickCanceled;
            }
        }

        private void OnDestroy()
        {
            if (_playerInput == null) return;

            var click = _playerInput.actions.FindAction("Click");

            if (click != null)
            {
                click.started -= OnClickStarted;
                click.canceled -= OnClickCanceled;
            }
        }

        private void Update()
        {
            if (!_isPressed) return;
            EventBus.Publish(new InputTouchMovedEvent { TouchPosition = GetTouchPosition() });
        }

        private void OnClickStarted(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return;
            _isPressed = true;
            EventBus.Publish(new InputTouchStartedEvent { TouchPosition = GetTouchPosition() });
        }

        private void OnClickCanceled(InputAction.CallbackContext ctx)
        {
            _isPressed = false;
            EventBus.Publish(new InputTouchEndedEvent { TouchPosition = GetTouchPosition() });
        }

        private Vector2 GetTouchPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return Touchscreen.current.primaryTouch.position.ReadValue();
            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();
            return Vector2.zero;
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                int fingerId = Touchscreen.current.primaryTouch.touchId.ReadValue();
                return EventSystem.current.IsPointerOverGameObject(fingerId);
            }

            return EventSystem.current.IsPointerOverGameObject(-1);
        }
    }
}
