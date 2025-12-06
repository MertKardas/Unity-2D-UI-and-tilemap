using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [ShowNonSerializedField]float volumeLevel;
    [SerializeField]private Slider volumeSlider;

    
    private void OnEnable()
    {
        
        volumeLevel = AudioManager.Instance.Volume;
        Debug.Log("1-Settings Menu Volume Level: " + volumeLevel);
        volumeSlider.value = Mathf.Clamp01(volumeLevel);
        Debug.Log("2-Settings Menu Volume Level: " + volumeLevel);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
     
        
    }
    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
    public void OnVolumeChanged(float value)
    {
        Debug.Log("Volume Slider Changed to: " + value);
        volumeLevel = Mathf.Clamp01(value);
        AudioManager.Instance.SetAndSaveVolume(volumeLevel);
    
        
       
    }
    public void SaveSettings(){
        AudioManager.Instance.SetAndSaveVolume(volumeLevel);
    }
}
