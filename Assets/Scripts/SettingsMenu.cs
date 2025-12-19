using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]private Slider volumeSlider;

    
    private void OnEnable()
    {
        Debug.Log("2-Settings Menu Volume Level: " + volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
     
        
    }
    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
    public void OnVolumeChanged(float value)
    {
        Debug.Log("Volume Slider Changed to: " + value);
        AudioManager.Instance.SetVolume(AudioType.Master, value);
    }
   
}
