using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]private Slider healthSlider;
    [SerializeField]private TextMeshProUGUI healtText;

    [SerializeField]private TextMeshProUGUI coinText;
    [SerializeField]private GameObject statsPanel;
    [SerializeField]private GameObject playerDeathPanel;
    [SerializeField] private GameObject pauseMenu; 
    private void Start() {
        playerController = Object.FindAnyObjectByType<PlayerController>();
        playerController.OnHealthChanged += UpdateHealthUI;
        playerController.OnCoinChanged += UpdateCoinUI;
        playerController.OnPlayerDeath += OpenPlayerDeathPanel;
        healthSlider.maxValue = playerController.Health;
        GameManager.Instance.OnGameover += CloseStatsPanel;
        InputManager.Instance.inputActions.UI.Cancel.performed += ctx => {
            if (pauseMenu.activeSelf) {
                ClosePauseMenu();
            } else {
                OpenPauseMenu();
            }
        };
        UpdateHealthUI(playerController.Health);
        UpdateCoinUI(playerController.Coin);
    }
    

    

    private void UpdateHealthUI(int currentHealth) {

        healthSlider.value = currentHealth;
        healtText.text =  currentHealth.ToString();
    }

    private void UpdateCoinUI(int currentCoin) {
        coinText.text =  currentCoin.ToString();
    }
    private void OpenPlayerDeathPanel() {
        playerDeathPanel.SetActive(true);

        LeanTween.alphaCanvas(playerDeathPanel.GetComponent<CanvasGroup>(), 1f, 2f).setEase(LeanTweenType.easeOutQuad);
       

    }
    public void OpenPauseMenu() {
        pauseMenu.SetActive(true);
        GameManager.Instance.PauseGame(); 
    }
    public void ClosePauseMenu() {
        pauseMenu.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
    public void CloseStatsPanel() { 
        // Faded out and close

        LeanTween.alphaCanvas(statsPanel.GetComponent<CanvasGroup>(), 0f, 1f).setEase(LeanTweenType.easeInQuad).setOnComplete(() => {
            statsPanel.SetActive(false);
        });
    }

    
}
