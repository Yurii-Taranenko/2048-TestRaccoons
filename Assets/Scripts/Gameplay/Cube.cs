using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Cube : MonoBehaviour
{
    [SerializeField] private List<TextMeshPro> _valueDisplay;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Renderer _renderer;

    private float _launchForce;
    private int _value;
    private int _id;
    private bool _isAlive = true;
    private CubeColorConfig _colorConfig;

    public int Value => _value;
    public int Id => _id;
    public bool IsAlive => _isAlive;
    public Rigidbody Rigidbody => _rigidbody;

    public void SetColorConfig(CubeColorConfig colorConfig)
    {
        if (_colorConfig == null)
            _colorConfig = colorConfig;
    }

    public void Initialize(GameplayConfig config, int id)
    {
        _id = id;
        _isAlive = true;
        _launchForce = config.cubeSpawnForce;
        _value = Random.value < config.spawnProbability2 ? 2 : 4;
        
        UpdateValueTexts();
        UpdateCubeColor();

        _rigidbody.isKinematic = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void Launch()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(Vector3.forward * _launchForce, ForceMode.Impulse);
    }

    public void MergeWith(int newValue)
    {
        _value = newValue;
        _isAlive = true;
        UpdateValueTexts();
        UpdateCubeColor();
    }

    private void UpdateCubeColor()
    {
        if (_renderer == null || _colorConfig == null)
            return;

        Color targetColor = _colorConfig.GetColorForValue(_value);
        Material mat = _renderer.material;
        mat.color = targetColor;
    }

    public void ApplyPostMergePhysics(Vector3 mergePosition)
    {
        _rigidbody.isKinematic = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.position = mergePosition;
    }

    public void Reset()
    {
        _value = 0;
        _isAlive = false;
        UpdateValueTexts();
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EventBus.Publish(new CubeCollisionRequestEvent
        {
            Collision = collision,
            InitiatorCubeId = _id
        });
    }

    private void UpdateValueTexts()
    {
        foreach (var textMesh in _valueDisplay)
            textMesh.text = _value.ToString();
    }
}
