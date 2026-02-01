using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler {

    [Header("Sounds")]
    [SerializeField] AudioData _hoverSound;
    [SerializeField] AudioData _pressedSound;

    [Header("Settings")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _tweenDuration = 0.2f; 
    private Vector3 _originalScale;

    protected Button _button;
    protected void Awake()
    {
        _button = GetComponent<Button>();
        _originalScale = transform.localScale;
    }
    

    protected virtual void OnEnable() {
        if (_button == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        _button.onClick.AddListener(ButtonOnClick);
        LeanTween.cancel(gameObject);
    }

    protected virtual void OnDisable() {
        LeanTween.cancel(gameObject);
        if (_button == null) return;
        _button.onClick.RemoveListener(ButtonOnClick);
    }

    public void OnPointerEnter(PointerEventData eventData) {
   
        if (_button == null || !_button.interactable) return;
        EventSystem.current.SetSelectedGameObject(gameObject);
       
    }

    public void OnPointerExit(PointerEventData eventData) {
    
        if (_button == null || !_button.interactable) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _originalScale, _tweenDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
    }

    public virtual void ButtonOnClick() {
        Debug.Log("Menu Button Clicked");
        if (_pressedSound != null)
            AudioManager.Instance.PlaySound(_pressedSound);
        EventSystem.current.SetSelectedGameObject(null);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _originalScale * _hoverScale, _tweenDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);

        if (_hoverSound != null)
            AudioManager.Instance.PlaySound(_hoverSound);
    }
}