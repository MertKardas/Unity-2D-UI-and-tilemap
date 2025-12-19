using UnityEngine;
using UnityEngine.Audio;

public class Ambience : MonoBehaviour
{
    [SerializeField] AudioData ambienceData; 
    private void Start() {
        AudioManager.Instance.PlaySound(ambienceData);
        GameManager.Instance.OnGameover += () => {
            AudioManager.Instance.StopAudioByType(AudioType.Ambience);
           
        };
        
    }
    

   
}
