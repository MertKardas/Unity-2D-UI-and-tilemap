using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]private PlayerController playerController;

    [SerializeField]private Slider healthSlider;
    [SerializeField]private TextMeshProUGUI healtText;

    [SerializeField]private TextMeshProUGUI coinText;

    [SerializeField]private GameObject playerDeathPanel;
    [SerializeField]private GameObject playerDeathMenu;
    private void Start() {
        playerController.OnHealthChanged += UpdateHealthUI;
        playerController.OnCoinChanged += UpdateCoinUI;
        playerController.OnPlayerDeath += OpenPlayerDeathPanel; 
        healthSlider.maxValue = playerController.Health; 
        UpdateHealthUI();
        UpdateCoinUI();
    }

    

    private void UpdateHealthUI() {

        healthSlider.value = playerController.Health;
        healtText.text =  playerController.Health.ToString();
    }

    private void UpdateCoinUI() {
        coinText.text =  playerController.Coin.ToString();
    }
    private void OpenPlayerDeathPanel() {
        playerDeathPanel.SetActive(true);

        LeanTween.alphaCanvas(playerDeathPanel.GetComponent<CanvasGroup>(), 1f, 2f).setEase(LeanTweenType.easeOutQuad);
       

    }
    
    

}
