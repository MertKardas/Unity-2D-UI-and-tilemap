using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class GameUI : MonoBehaviour {
    //Backing fields for UI Panels
    public StatsPanelUI StatsPanel;
    public DeathMenuUI PlayerDeathPanel;
    public PauseMenu PauseMenu;
    public SettingsMenu SettingsPanel;

    private void OnEnable() {
        GameManager.Instance.OnGameover += OnGameover;
        GameManager.Instance.OnGameStarted += OnGameStarted;
        InputManager.Instance.Subscribe(InputType.Cancel, OnCancel, InputActionPhase.Performed);
      

    }
    private void OnDisable() {
        if(GameManager.Instance != null) {
            GameManager.Instance.OnGameover -= OnGameover;
            GameManager.Instance.OnGameStarted -= OnGameStarted;
        }
        if(InputManager.Instance != null)
            InputManager.Instance.Unsubscribe(InputType.Cancel, OnCancel, InputActionPhase.Performed);
    }
    //Cancel Input Handler
    void OnCancel(InputAction.CallbackContext ctx) {
        if (!ctx.performed)
            return;
        Debug.Log("Cancel input received in GameUI");
        if (PauseMenu.gameObject.activeSelf) {
            InputManager.Instance.DisablePlayerInput();
            GameManager.Instance.ResumeGame();
            PauseMenu.gameObject.SetActive(false);

        } else if (SettingsPanel.gameObject.activeSelf) {
            SwitchPanel(SettingsPanel.gameObject, PauseMenu.gameObject);
        } else {
            SwitchPanel(null, PauseMenu.gameObject);
            GameManager.Instance.PauseGame();
            
        }
    }

    private void OnGameover() {
        SwitchPanel(StatsPanel.gameObject, PlayerDeathPanel.gameObject);
    }

    private void OnGameStarted() {
        SwitchPanel(PlayerDeathPanel.gameObject, StatsPanel.gameObject);
    }

    public void SwitchPanel(GameObject fromPanel, GameObject toPanel) {
        if(fromPanel == toPanel)
            return;
        if(fromPanel!= null)
            fromPanel.SetActive(false);
        if(toPanel != null)
            toPanel.SetActive(true);
    }

    
}
