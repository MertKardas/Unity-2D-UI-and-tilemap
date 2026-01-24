using MyUtility;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    #region Events
    public event Action OnGamePaused;
    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public event Action OnGameover;
    #endregion

    #region Properties
    public GameState CurrentGameState { get; private set; }
    #endregion

    #region Unity Lifecycle
    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    #endregion

    #region Game State Management
    public void PauseGame() {
        InputManager.Instance.DisablePlayerInput();
        Time.timeScale = 0f;
        CurrentGameState = GameState.Paused;
        OnGamePaused?.Invoke();
    }

    public void UnpauseGame() {
        InputManager.Instance.EnablePlayerInput();
        Time.timeScale = 1f;
        CurrentGameState = GameState.Playing;
        OnGameStarted?.Invoke();
    }

    public void Gameover() {
        Time.timeScale = 0f;
        CurrentGameState = GameState.Gameover;
        AudioManager.Instance.StopAudioByType(AudioType.Music);
        AudioManager.Instance.StopAudioByType(AudioType.Ambience);
        OnGameover?.Invoke();
    }

    public void EndGame() {
        Time.timeScale = 0f;
        CurrentGameState = GameState.EndGame;
        OnGameEnded?.Invoke();

        var gameUI = FindAnyObjectByType<GameUI>();
        gameUI.FadeOutScene().setOnComplete(() => SceneManager.LoadScene("EndGame"));
    }
    #endregion

    #region Scene Management
    public async void RestartGame() {
        var result = SaveManager.Instance.QuickLoad();
        if (!result.Success) {
            Debug.LogWarning("Failed to load last save: " + result.ErrorMessage);
            SaveManager.Instance.CurrentGameSaveData = null;
            await SceneManager.LoadSceneAsync(0);
        }
    }

    public void ReturnToMainMenu() {
        AudioManager.Instance.StopAllAudio();
        SceneManager.LoadScene("MainMenu");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode) {
        InputManager.Instance.EnablePlayerInput();
        Time.timeScale = 1f;

        if (scene.name == "MainMenu") {
            SaveManager.Instance.CurrentGameSaveData = null;
        } else {
            OnGameStarted?.Invoke();
        }
    }
    #endregion
}

public enum GameState {
    Playing,
    Paused,
    Gameover,
    EndGame
}

