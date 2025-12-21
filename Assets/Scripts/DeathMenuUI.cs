using UnityEngine;

public class DeathMenuUI : MonoBehaviour
{
    public void RestartLevel()
    {
        GameManager.Instance.RestartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void OnEnable()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        //openinig alpha animation
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f)
            .setEase(LeanTweenType.easeOutQuad)
            .setIgnoreTimeScale(true);

    }

    
}
