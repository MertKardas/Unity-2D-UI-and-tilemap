using MyUtility;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks; // Add this at the top if not already present
public class GameManager : Singleton<GameManager> {
    public Action OnGamePaused;
    public Action OnGameStarted;
    public Action OnGameover;
    public GameState CurrentGameState { get; private set; }
    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += SceneLoaded;

    }
    protected override void OnDestroy() {
        base.OnDestroy();
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public void PauseGame() {
        InputManager.Instance.DisablePlayerInput();
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
        CurrentGameState = GameState.Paused;
    }
    public void UnpauseGame() {
        InputManager.Instance.EnablePlayerInput();
        Time.timeScale = 1f;
        OnGameStarted?.Invoke();
        CurrentGameState = GameState.Playing;
    }

    public void Gameover() {
        OnGameover?.Invoke();
        Time.timeScale = 0f;
        AudioManager.Instance.StopAudioByType(AudioType.Music);
        AudioManager.Instance.StopAudioByType(AudioType.Ambience);
        CurrentGameState = GameState.Gameover;
    }
    public async void RestartGame() {
        var result = SaveManager.Instance.QuickLoad();
        if (!result.Success) {
            Debug.LogWarning("Failed to load last save: " + result.ErrorMessage);
            //clear save data and load first scene
            SaveManager.Instance.CurrentGameSaveData = null;
            await SceneManager.LoadSceneAsync(0);
        }//successful load 
        else {
            var lastScene = SaveManager.Instance.CurrentGameSaveData.SceneName;
            await SceneManager.LoadSceneAsync(lastScene);
        }
    }

    
    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        InputManager.Instance.EnablePlayerInput();
        Time.timeScale = 1f;
        if (scene.name != "MainMenu")
            OnGameStarted?.Invoke();
        else {
            SaveManager.Instance.CurrentGameSaveData = null;
        }
    }
    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        
        AudioManager.Instance.StopAllAudio();
    }
    
}
public enum GameState {
    Playing,
    Paused,
    Gameover
}

