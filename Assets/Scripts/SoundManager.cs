using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }   
}
