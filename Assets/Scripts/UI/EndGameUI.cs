using UnityEngine;
using UnityEngine.UI;
public class EndGameUI : MonoBehaviour
{
    Button _returnToMainMenuButton;
    private void OnEnable()
    {
        _returnToMainMenuButton = GetComponentInChildren<Button>();
        _returnToMainMenuButton.onClick.AddListener(OnReturnToMainMenuClicked);
    }
    private void OnDisable()
    {
        _returnToMainMenuButton.onClick.RemoveListener(OnReturnToMainMenuClicked);
    }
    private void OnReturnToMainMenuClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
