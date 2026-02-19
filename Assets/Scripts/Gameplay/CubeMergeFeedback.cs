using UnityEngine;

/// <summary>
/// Manages visual and audio feedback for cube merges.
/// Decouples presentation logic from merge system.
/// </summary>
public class CubeMergeFeedback : MonoBehaviour
{
    [SerializeField] private ParticleSystem mergePrefab;
    [SerializeField] private AudioClip mergeSound;
    [SerializeField] private float feedbackDuration = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (mergeSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = mergeSound;
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<CubeMergedEvent>(OnCubeMerged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CubeMergedEvent>(OnCubeMerged);
    }

    private void OnCubeMerged(CubeMergedEvent evt)
    {
        PlayMergeFeedback(evt.MergePosition, evt.ResultingValue);
    }

    private void PlayMergeFeedback(Vector3 position, int resultValue)
    {
        PlayParticleEffect(position);
        PlayMergeSound();
        
        Debug.Log($"[CubeMergeFeedback] Merge feedback at {position} for value {resultValue}");
    }

    private void PlayParticleEffect(Vector3 position)
    {
        if (mergePrefab == null)
            return;

        var particles = Instantiate(mergePrefab, position, Quaternion.identity);
        Destroy(particles.gameObject, feedbackDuration);
    }

    private void PlayMergeSound()
    {
        if (_audioSource == null)
            return;

        _audioSource.PlayOneShot(_audioSource.clip);
    }
}
