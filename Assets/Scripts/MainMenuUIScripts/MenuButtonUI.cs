using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [Header("Sounds")]
    [SerializeField] AudioData _hoverSound;
    [SerializeField] AudioData _pressedSound;

    [SerializeField] private Color _hoverColor = Color.yellow;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationSpeed = 5f;

    private Vector3 _originalScale;
    private Color _originalColor;
    protected Button _button;
    public void OnPointerEnter(PointerEventData eventData) {
        LeanTween.scale(gameObject, _originalScale * _hoverScale, 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
        LeanTween.color(gameObject, _hoverColor, 0.2f)
            .setEase(LeanTweenType.easeOutQuad)
            .setIgnoreTimeScale(true);
        if (_hoverSound != null)
            AudioManager.Instance.PlaySound(_hoverSound);
       
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Animation scale down
        LeanTween.scale(gameObject, _originalScale, 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
        LeanTween.color(gameObject, _originalColor, 0.2f)
            .setEase(LeanTweenType.easeOutQuad)
            .setIgnoreTimeScale(true);
       
    }

    protected virtual void Awake() {
        _originalScale = transform.localScale;
        _originalColor = Color.white;
        
    }
    protected virtual void OnEnable() {
        transform.localScale = _originalScale;
        GetComponent<Image>().color = _originalColor;
        LeanTween.cancel(gameObject);
    }
    
    public virtual void ButtonOnClick() {
        Debug.Log("Menu Button Clicked");
        if (_pressedSound != null)
           AudioManager.Instance.PlaySound(_pressedSound);
    }
   


}
