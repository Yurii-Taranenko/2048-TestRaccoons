using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input and translates it to game events.
/// Decouples input device handling from game logic.
/// </summary>
public class InputManager : BaseManager
{
    private PlayerInput _playerInput;
    private InputActionAsset _inputActions;

    public override void Init()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("[InputManager] PlayerInput component not found!");
            return;
        }

        _inputActions = _playerInput.actions;
        SubscribeToInputActions();
    }

    private void SubscribeToInputActions()
    {
        // Subscribe to touch/drag input
        _inputActions["Touch"].started += OnTouchStarted;
        _inputActions["Touch"].performed += OnTouchPerformed;
        _inputActions["Touch"].canceled += OnTouchCanceled;
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions["Touch"].started -= OnTouchStarted;
            _inputActions["Touch"].performed -= OnTouchPerformed;
            _inputActions["Touch"].canceled -= OnTouchCanceled;
        }
    }

    /// <summary>
    /// Fired when player touches/clicks the screen.
    /// </summary>
    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = Touchscreen.current.position.ReadValue();
        EventBus.Publish(new InputTouchStartedEvent { TouchPosition = touchPosition });
    }

    /// <summary>
    /// Fired while player is dragging.
    /// </summary>
    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = Touchscreen.current.position.ReadValue();
        EventBus.Publish(new InputTouchMovedEvent { TouchPosition = touchPosition });
    }

    /// <summary>
    /// Fired when player releases touch.
    /// </summary>
    private void OnTouchCanceled(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = Touchscreen.current.position.ReadValue();
        EventBus.Publish(new InputTouchEndedEvent { TouchPosition = touchPosition });
    }
}