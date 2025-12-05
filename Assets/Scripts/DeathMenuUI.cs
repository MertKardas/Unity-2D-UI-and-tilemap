using UnityEngine;

public class DeathMenuUI : MonoBehaviour
{
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void OnEnable()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f);
    }

    
}
