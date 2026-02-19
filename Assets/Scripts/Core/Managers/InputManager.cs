using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input and translates it to game events.
/// </summary>
public class InputManager : BaseManager
{
    private PlayerInput _playerInput;

    public override void Init()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("[InputManager] PlayerInput component not found!");
            return;
        }

        SubscribeToInputActions();
    }

    private void SubscribeToInputActions()
    {
        var clickAction = _playerInput.actions.FindAction("Click");
        var touchAction = _playerInput.actions.FindAction("Touch");
        
        if (clickAction == null)
        {
            Debug.LogError("[InputManager] Click action not found!");
            return;
        }
        
        if (touchAction == null)
        {
            Debug.LogError("[InputManager] Touch action not found!");
            return;
        }

        clickAction.started += OnClickStarted;
        clickAction.canceled += OnClickCanceled;
        touchAction.performed += OnTouchMoved;
    }

    private void OnDestroy()
    {
        if (_playerInput != null)
        {
            var clickAction = _playerInput.actions.FindAction("Click");
            var touchAction = _playerInput.actions.FindAction("Touch");
            
            if (clickAction != null)
            {
                clickAction.started -= OnClickStarted;
                clickAction.canceled -= OnClickCanceled;
            }
            
            if (touchAction != null)
            {
                touchAction.performed -= OnTouchMoved;
            }
        }
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = GetTouchPosition();
        EventBus.Publish(new InputTouchStartedEvent { TouchPosition = touchPosition });
    }

    private void OnTouchMoved(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = GetTouchPosition();
        EventBus.Publish(new InputTouchMovedEvent { TouchPosition = touchPosition });
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = GetTouchPosition();
        EventBus.Publish(new InputTouchEndedEvent { TouchPosition = touchPosition });
    }

    private Vector2 GetTouchPosition()
    {
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        if (Touchscreen.current != null)
            return Touchscreen.current.position.ReadValue();

        return Vector2.zero;
    }
}