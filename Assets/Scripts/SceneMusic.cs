using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField]private AudioClip _musicData;
       void Start()
    {
        AudioManager.Instance.PlayMusic(_musicData);    
    }
    void OnDestroy()
    {
        float fadeOutDuration = 2f; // Duration of the fade-out in seconds
        if(AudioManager.Instance != null)
            AudioManager.Instance.FadeOutMusic(fadeOutDuration);
    }
}
