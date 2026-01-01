using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using static System.Net.WebRequestMethods;
using DentedPixel;
public class PauseMenu : MonoBehaviour,IGamePanel
{
    [NaughtyAttributes.Scene, SerializeField] public string mainMenuSceneName;
    private GameUI gameUI;
    private CanvasGroup canvasGroup;
    [SerializeField] Button selectedButton;
    private void Awake() {
       
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
       
    }
    private void OnEnable() {
        if(selectedButton!= null) 
            EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
         InputManager.Instance.Subscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);
        
    }

    #region Button Methods
    public void OpenSocialLink(string url) { 
        try {
            Application.OpenURL(url);
        } catch (System.Exception e) {
            UnityEngine.Debug.LogError("Failed to open URL: " + e.Message);
        }
    }
    public void ReturnMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
    public void QuitGame() {
        Time.timeScale = 1f; // Reset time scale

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    public void PauseToSettings()
    {
        gameUI.SwitchPanel(gameObject, gameUI.SettingsPanel.gameObject, gameUI.panelTransition);
    }
#endregion

    void IGamePanel.SetPanelController(GameUI controller) {
        gameUI = controller;
    }
    private void OnDisable() {
        LeanTween.cancel(this.gameObject);
        if(InputManager.Instance != null)   
            InputManager.Instance.Unsubscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);
    }
    private void OnCancel(InputAction.CallbackContext ctx) {
        if (!ctx.started) return;
        var sequence = gameUI.SwitchPanel(gameObject, gameUI.StatsPanel.gameObject, gameUI.panelTransition);
        sequence.append(() => {
            gameUI.backgroundPanel.SetActive(false);
        });
    }


}

