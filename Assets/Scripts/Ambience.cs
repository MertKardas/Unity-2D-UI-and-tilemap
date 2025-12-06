using UnityEngine;

public class Ambience : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = AudioManager.Instance.Volume;
        GameManager.Instance.OnGameover += () => {
            audioSource.Pause();
        };
        
    }
    private void OnEnable() {
        AudioManager.Instance.OnVolumeChanged += UpdateVolume;
    }
    private void OnDisable() {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float volume) {
        audioSource.volume = volume;
    }
}
