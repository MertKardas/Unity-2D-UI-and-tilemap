using UnityEngine;


/// <summary>
/// Simple highlight effect that scales the object up and down in a ping-pong pattern when enabled.
/// </summary>
public class SquareHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeInOutSine;
    
    private Vector3 _originalScale;
    private LTDescr _highlightTween;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartHighlight();
    }

    private void OnDisable()
    {
        StopHighlight();
    }

    private void StartHighlight()
    {
 
        // Reset to original scale first
        transform.localScale = _originalScale;
        
        // Create ping-pong scale tween
        _highlightTween = LeanTween.scale(gameObject, _originalScale * scaleMultiplier, duration)
            .setEase(easeType)
            .setLoopPingPong()
            .setIgnoreTimeScale(true); 
    }

    private void StopHighlight()
    {
        // Kill tween and reset scale
        LeanTween.cancel(_highlightTween.id);
        transform.localScale = _originalScale;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(_highlightTween.id);
    }
}