using UnityEngine;

public class FootStep : MonoBehaviour
{
    PlayerController controller;
    private AudioSource audioSource;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponentInParent<PlayerController>();
    }
    private void Update() {
        if (controller.Movement !=Vector2.zero && !audioSource.isPlaying) {
            audioSource.Play();
        } else if (controller.Movement == Vector2.zero && audioSource.isPlaying) {
            audioSource.Pause();
        }
    }
}
