using UnityEngine;

public class DeathMenuUI : MonoBehaviour,IGamePanel
{
    GameUI gameUI;
    public void RestartLevel()
    {
        GameManager.Instance.RestartGame();
    }
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    void IGamePanel.SetPanelController(GameUI controller) {
        gameUI = controller;
    }


    private void OnDisable() {
        LeanTween.cancel(this.gameObject);
    }

}

