using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour {
    // Inspector'dan atarsan Find ile uðraþmazsýn, performans artar.
    [SerializeField] private PlayerController playerController;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText; // Yazým hatasý düzeltildi (healt -> health)
    [SerializeField] private TextMeshProUGUI coinText;

    HealthComponent healthComp; 
    private void Awake() {

        // Eðer inspector'dan atanmadýysa, oyun baþýnda bir kez bul.
        if (playerController == null) {
            playerController = FindAnyObjectByType<PlayerController>();
            if (playerController == null)
                Debug.LogError("PlayerController not found in the scene!");
            return; 
        }
        

    }
 
    private void OnEnable() {
        if (playerController != null) {
            // Initial setup
            healthComp = playerController.HealthComponent;
            healthSlider.maxValue = healthComp.MaxHealth;
            UpdateHealthUI(healthComp.Health, true); 
            //UpdateCoinUI(playerController.Coin);

   
            healthComp.OnHealthChanged += OnHealthChangedHandler;
            //playerController.OnCoinChanged += UpdateCoinUI;
        }
    }

    private void OnDisable() {
       
        LeanTween.cancel(gameObject);

        //(Null Check)
        if (playerController != null) {
            healthComp.OnHealthChanged -= OnHealthChangedHandler;
            //playerController.OnCoinChanged -= UpdateCoinUI;
        }
    }

   
    private void OnHealthChangedHandler(int currentHealth) {
        UpdateHealthUI(currentHealth, true);
    }

    private void UpdateHealthUI(int currentHealth, bool animate) {
        
        LeanTween.cancel(gameObject);

        healthText.SetText($"{currentHealth} / {healthComp.MaxHealth}");

        if (animate) {
            float startValue = healthSlider.value;
            LeanTween.value(gameObject, startValue, currentHealth, 0.5f)
                .setEase(LeanTweenType.easeOutCubic) // Biraz yumuþak geçiþ ekledim
                .setOnUpdate((float val) => {
                    healthSlider.value = val;
                });
        } else {
            healthSlider.value = currentHealth;
        }
    }

    private void UpdateCoinUI(int currentCoin) {
        coinText.text = currentCoin.ToString();
    }
}