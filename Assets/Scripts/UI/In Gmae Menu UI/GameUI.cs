
using UnityEngine;
using UnityEngine.InputSystem;
public class GameUI : MonoBehaviour
{
    //Backing fields for UI Panels
    public StatsPanelUI StatsPanel;
    public DeathMenuUI PlayerDeathPanel;
    public PauseMenu PauseMenu;
    public SettingsMenu SettingsPanel;
    public GameObject backgroundPanel;
    public GameObject EndgGamePanel;
    public UITransition panelTransition;
    private void OnEnable()
    {
        GameManager.Instance.OnGameover += OnGameover;
        GameManager.Instance.OnGameStarted += OnGameStarted;
        InputManager.Instance.Subscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);
        (PlayerDeathPanel as IGamePanel).SetPanelController(this);
        (PauseMenu as IGamePanel).SetPanelController(this);
        (SettingsPanel as IGamePanel).SetPanelController(this);
    }
    private void Start()
    {
        StatsPanel.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameover -= OnGameover;
            GameManager.Instance.OnGameStarted -= OnGameStarted;
        }
        if (InputManager.Instance != null)
            InputManager.Instance.Unsubscribe(InputType.Cancel, OnCancel, InputActionPhase.Started);
    }
    //Cancel Input Handler
    void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (!PauseMenu.gameObject.activeInHierarchy && !backgroundPanel.gameObject.activeInHierarchy)
        {
            backgroundPanel.SetActive(true);
            SwitchPanel(StatsPanel.gameObject, PauseMenu.gameObject, panelTransition);
        }
        else if (PauseMenu.gameObject.activeInHierarchy && backgroundPanel.gameObject.activeInHierarchy)
        {
            SwitchPanel(null, StatsPanel.gameObject, panelTransition);
            backgroundPanel.SetActive(true);
        }
    }
    private void OnGameover()
    {
        backgroundPanel.SetActive(true);
        if (PauseMenu.gameObject.activeInHierarchy)
        {
            //close pause menu first
            SwitchPanel(PauseMenu.gameObject, PlayerDeathPanel.gameObject, panelTransition);
            return;
        }
        else if (SettingsPanel.gameObject.activeInHierarchy)
        {
            //close settings menu first
            SwitchPanel(null, PlayerDeathPanel.gameObject, panelTransition);
            return;
        }
        else
        {
            SwitchPanel(StatsPanel.gameObject, PlayerDeathPanel.gameObject, panelTransition);
        }
    }

    private void OnGameStarted()
    {
        backgroundPanel.SetActive(false);
    }

    public LTSeq SwitchPanel(GameObject fromPanel, GameObject toPanel, UITransition panelTransition)
    {
        if (panelTransition == null)
        {
            Debug.LogWarning("UITransition is not assigned");
            return LeanTween.sequence();
        }

        var sequence = panelTransition.Transition(fromPanel, toPanel);
        return sequence;
    }
    public LTDescr FadeOutScene()
    {
        EndgGamePanel.SetActive(true);
        var canvasGroup = EndgGamePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup is not assigned to backgroundPanel");
            return null;
        }
        canvasGroup.alpha = 0f;
        var fadeOutTime = 1f;
        var fadeOut = LeanTween
            .alphaCanvas(canvasGroup, 1f, fadeOutTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true);
        return fadeOut;
    }

}

