using NaughtyAttributes;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenu : MonoBehaviour, IGamePanel {
    [BoxGroup("Volume Sliders"), SerializeField]
    private Slider 
        masterVolumeSlider,
        musicVolumeSlider,
        sfxVolumeSlider,
        ambienceVolumeSlider,
        uiVolumeSlider;
    GameUI gameUI; 
    CanvasGroup canvasGroup;
    private void Awake() {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        ambienceVolumeSlider.onValueChanged.AddListener(OnAmbienceVolumeChanged);
        uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
        canvasGroup.interactable = false;
        InputManager.Instance.Subscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);
    }
    void IGamePanel.SetPanelController(GameUI controller) {
        gameUI = controller;
    }
    private void OnDisable()
    {
        
        masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        ambienceVolumeSlider.onValueChanged.RemoveListener(OnAmbienceVolumeChanged);
        uiVolumeSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);
        LeanTween.cancel(this.gameObject);
        InputManager.Instance.Unsubscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);

    }
    #region Volume Change Handlers
    public void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioType.Master, value);
    }
    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioType.Music, value);
    }
    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioType.SFX, value);
    }
    public void OnAmbienceVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioType.Ambience, value);
    }
    public void OnUIVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioType.UI, value);
    }

    #endregion
    
    public void OnReturnClicked() {
        gameUI.SwitchPanel(gameUI.SettingsPanel.gameObject, gameUI.PauseMenu.gameObject, gameUI.panelTransition);
    }
    private void OnCancel(InputAction.CallbackContext ctx) {
        if (!ctx.started) return;
        OnReturnClicked();
    }


}
