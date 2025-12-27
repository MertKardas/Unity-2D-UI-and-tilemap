using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [Header("Sounds")]
    [SerializeField] AudioData _hoverSound;
    [SerializeField] AudioData _pressedSound;

    [Header("Settings")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _tweenDuration = 0.2f; 

    private Vector3 _originalScale;
    protected Button _button;

    protected virtual void Start() {
      
        _originalScale = transform.localScale;

        if (_button == null)
            _button = GetComponent<Button>();

        if (_button == null)
            Debug.LogError($"MenuButtonUI on {gameObject.name} requires a Button component!");
    }

    protected virtual void OnEnable() {
        if (_button == null) return;

        transform.localScale = _originalScale;

        _button.onClick.AddListener(ButtonOnClick);
        LeanTween.cancel(gameObject);
    }

    protected virtual void OnDisable() {
        if (_button == null) return;

        LeanTween.cancel(gameObject);
        _button.onClick.RemoveListener(ButtonOnClick);
    }

    public void OnPointerEnter(PointerEventData eventData) {
   
        if (_button == null || !_button.interactable) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _originalScale * _hoverScale, _tweenDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);

        if (_hoverSound != null)
            AudioManager.Instance.PlaySound(_hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Exit animasyonu her durumda çalýþabilir (takýlý kalmamasý için)
        // Ancak yine de interactable kontrolü tutarlýlýk için eklenebilir.
        if (_button == null || !_button.interactable) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _originalScale, _tweenDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
    }

    public virtual void ButtonOnClick() {
        // Interactable kontrolünü Unity zaten onClick için kendi içinde yapýyor, buraya gerek yok.
        Debug.Log("Menu Button Clicked");
        if (_pressedSound != null)
            AudioManager.Instance.PlaySound(_pressedSound);
        EventSystem.current.SetSelectedGameObject(null);
    }
}