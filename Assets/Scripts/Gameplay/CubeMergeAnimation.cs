using System.Collections;
using UnityEngine;

/// <summary>
/// Handles merge animation and physics for merged cubes.
/// Smoothly combines two cubes into one at merge position.
/// </summary>
public class CubeMergeAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);

    private Cube _cube;
    private Vector3 _initialScale;

    private void Start()
    {
        _cube = GetComponent<Cube>();
        _initialScale = transform.localScale;
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
        if (_cube == null || !_cube.IsAlive)
            return;

        if (Vector3.Distance(transform.position, evt.MergePosition) < 0.1f)
        {
            StartCoroutine(AnimateMerge());
        }
    }

    private IEnumerator AnimateMerge()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;
            float scale = scaleCurve.Evaluate(progress);

            transform.localScale = startScale * scale;

            yield return null;
        }

        transform.localScale = startScale;
    }
}
