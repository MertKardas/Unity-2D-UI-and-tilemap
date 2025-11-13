using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    [SerializeField] AudioClip _hoverSound;
    [SerializeField] AudioClip _pressedSound;
    [SerializeField] private Color _hoverColor = Color.yellow;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationSpeed = 5f;
    [SerializeField] private AudioSource _audioSource;
    private Vector3 _originalScale;
    protected Button _button;
    public void OnPointerEnter(PointerEventData eventData) {
        //Animation scale up
        LeanTween.scale(gameObject, _originalScale * _hoverScale, 0.2f).setEase(LeanTweenType.easeOutBack);
        if (_hoverSound != null )
           _audioSource.PlayOneShot(_hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Animation scale down
        LeanTween.scale(gameObject, _originalScale, 0.2f).setEase(LeanTweenType.easeOutBack);
       
    }

    protected virtual void Awake() {
        _originalScale = transform.localScale;
      
  
    }
    public virtual void ButtonOnClick() {
        Debug.Log("Menu Button Clicked");
        if (_pressedSound != null)
           _audioSource.PlayOneShot(_pressedSound);
    }
   


}
