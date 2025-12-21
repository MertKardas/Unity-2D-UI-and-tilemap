using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    //Backing fields for UI Panels


    public StatsPanelUI StatsPanel;
    public DeathMenuUI PlayerDeathPanel;
    public PauseMenu PauseMenu;
    public SettingsMenu SettingsPanel;

    private void Start() {

        InputManager.Instance.inputActions.UI.Cancel.performed += OnCancelInput;
        GameManager.Instance.OnGameover += OnGameover;
        GameManager.Instance.OnGameStarted += OnGameStarted;
    }
    private void OnDestroy() {
        InputManager.Instance.inputActions.UI.Cancel.performed -= OnCancelInput;
        GameManager.Instance.OnGameover -= OnGameover;
        GameManager.Instance.OnGameStarted -= OnGameStarted;
    }
    private void OnCancelInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        if (PauseMenu.gameObject.activeSelf) {
            InputManager.Instance.inputActions.Player.Enable();
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
