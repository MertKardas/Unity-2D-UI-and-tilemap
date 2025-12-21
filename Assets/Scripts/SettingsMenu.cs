using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [BoxGroup("Volume Sliders"), SerializeField]
    private Slider 
        masterVolumeSlider,
        musicVolumeSlider,
        sfxVolumeSlider,
        ambienceVolumeSlider,
        uiVolumeSlider;

    private void OnEnable()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        ambienceVolumeSlider.onValueChanged.AddListener(OnAmbienceVolumeChanged);
        uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
    }
    private void OnDisable()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        ambienceVolumeSlider.onValueChanged.RemoveListener(OnAmbienceVolumeChanged);
        uiVolumeSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);
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
    public void ReturnToPauseMenu()
    {
        GameUI gameUI = FindAnyObjectByType<GameUI>();
        gameUI.SwitchPanel(this.gameObject, gameUI.PauseMenu.gameObject);
    }

}
