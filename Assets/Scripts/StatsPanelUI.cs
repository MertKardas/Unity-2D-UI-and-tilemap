using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healtText;
    [SerializeField] private TextMeshProUGUI coinText;

    private bool isQuitting = false;
    private void OnEnable()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerController.OnHealthChanged += UpdateHealthUI;
        playerController.OnCoinChanged += UpdateCoinUI;
        
        
    }
    private void Start()
    {
        healthSlider.maxValue = playerController.Health;
        UpdateHealthUI(playerController.Health);
        UpdateCoinUI(playerController.Coin);
    }
    private void OnDisable() {
        playerController.OnHealthChanged -= UpdateHealthUI;
        playerController.OnCoinChanged -= UpdateCoinUI;
        if (isQuitting) return;
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0f, 3f)
           .setEase(LeanTweenType.easeInQuad)
           .setIgnoreTimeScale(true)
           .setOnComplete(() => gameObject.SetActive(false));
    }
    private void OnApplicationQuit() {
        isQuitting = true;
    }

    private void UpdateHealthUI(int currentHealth)
    {
        healthSlider.value = currentHealth;
        healtText.text = currentHealth.ToString();
    }

    private void UpdateCoinUI(int currentCoin)
    {
        coinText.text = currentCoin.ToString();
    }


    
}
