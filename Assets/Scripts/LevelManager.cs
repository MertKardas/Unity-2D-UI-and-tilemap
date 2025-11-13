using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    static LevelManager Instance;
    PlayerController playerController;
    private void Awake() => Instance = this;
    private void Start() {
        playerController = FindAnyObjectByType<PlayerController>();
        playerController.OnPlayerDeath += HandlePlayerDeath;
        
    }
    private void OnDestroy() {
        if (playerController != null) {
            playerController.OnPlayerDeath -= HandlePlayerDeath;
        }
    }
    private void HandlePlayerDeath() {

        playerController.enabled = false;
    }
    public void LoadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   
    }
}
