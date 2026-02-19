using UnityEngine;

public class CubeInputHandler : MonoBehaviour
{
    private Cube _activeCube;
    private Vector3 _cubeStartPosition;
    [SerializeField] private float maxHorizontalPosition = 1.2f;
    private bool _isDragging;
    
    private CubeManager _cubeManager;

    public void Initialize(CubeManager cubeManager)
    {
        _cubeManager = cubeManager;
        if (_cubeManager == null)
            Debug.LogError("[CubeInputHandler] CubeManager is null");
    }

    private void OnEnable()
    {
        EventBus.Subscribe<InputTouchStartedEvent>(OnTouchStarted);
        EventBus.Subscribe<InputTouchMovedEvent>(OnTouchMoved);
        EventBus.Subscribe<InputTouchEndedEvent>(OnTouchEnded);
        EventBus.Subscribe<ActiveCubeChangedEvent>(OnActiveCubeChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<InputTouchStartedEvent>(OnTouchStarted);
        EventBus.Unsubscribe<InputTouchMovedEvent>(OnTouchMoved);
        EventBus.Unsubscribe<InputTouchEndedEvent>(OnTouchEnded);
        EventBus.Unsubscribe<ActiveCubeChangedEvent>(OnActiveCubeChanged);
    }

    private void OnActiveCubeChanged(ActiveCubeChangedEvent evt)
    {
        if (_cubeManager != null)
            _activeCube = _cubeManager.GetCubeByIdPublic(evt.CubeId);
    }

    private void OnTouchStarted(InputTouchStartedEvent evt)
    {
        if (_activeCube == null)
            return;

        _cubeStartPosition = _activeCube.transform.position;
        _isDragging = true;
        _activeCube.Rigidbody.isKinematic = true;
    }

    private void OnTouchMoved(InputTouchMovedEvent evt)
    {
        if (!_isDragging || _activeCube == null)
            return;

        Vector3 screenPos = new Vector3(evt.TouchPosition.x, evt.TouchPosition.y, Camera.main.transform.position.z - _cubeStartPosition.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPos);

        float clampedX = Mathf.Clamp(-worldPosition.x, -maxHorizontalPosition, maxHorizontalPosition);

        Vector3 newPosition = _cubeStartPosition;
        newPosition.x = clampedX;
        _activeCube.transform.position = newPosition;
    }

    private void OnTouchEnded(InputTouchEndedEvent evt)
    {
        if (!_isDragging || _activeCube == null)
            return;

        _isDragging = false;
        _activeCube.Rigidbody.isKinematic = false;
        _activeCube.Launch();

        EventBus.Publish(new CubeLaunchEvent { Force = 1f });
    }
}