
using MyUtility;
using UnityEditor;
using UnityEngine;
using System;
public class AudioManager :Singleton<AudioManager> {
    
    AudioSource audioSource;
    public Action<float> OnVolumeChanged;
    [Range(0f, 1f)]
    public float Volume { get; set; } = 1.0f;
    protected override void Awake() {
        base.Awake();
        GameObject audioGameObject = new GameObject("AudioSource");
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.transform.SetParent(this.transform);
        audioSource = audioGameObject.GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("volume")) {
            PlayerPrefs.SetFloat("volume", Volume);
        } else {
            Volume = PlayerPrefs.GetFloat("volume");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }   
    public void SetVolume(float value)
    {
        Volume = value;
        audioSource.volume = Volume;
        PlayerPrefs.SetFloat("volume", Volume);
    }
}
