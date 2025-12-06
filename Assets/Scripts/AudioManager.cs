
using MyUtility;
using UnityEditor;
using UnityEngine;
using System;
public class AudioManager :Singleton<AudioManager> {
    
    AudioSource audioSource;
    public Action<float> OnVolumeChanged;

    [Header("Volume"), Range(0f, 1f)]
    public float Volume { get; private set;}
    public float DefaultVolume = 1f;
    public string VolumePrefKey = "masterVolume";

    protected override void Awake() {
        base.Awake();
        
        GameObject audioGameObject = new GameObject("AudioSource");
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.transform.SetParent(this.transform);
        audioSource = audioGameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey(VolumePrefKey)) {
            Volume = PlayerPrefs.GetFloat(VolumePrefKey, DefaultVolume);
            Debug.Log("Loaded volume: " + Volume);
        } else {
            PlayerPrefs.SetFloat(VolumePrefKey, DefaultVolume);
        }
        GameManager.Instance.OnGameover += () => {
            audioSource.volume = 0;
        };
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) => {
            audioSource.volume = Volume;
        };
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }   
    public void SetAndSaveVolume(float value)
    {
        Volume = value;
        audioSource.volume = Volume;
        PlayerPrefs.SetFloat(VolumePrefKey, Volume);
        PlayerPrefs.Save();
        OnVolumeChanged?.Invoke(Volume);
    }
}
