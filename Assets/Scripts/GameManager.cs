using UnityEngine;
using MyUtility;
using System;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public Action OnGamePaused;
    public Action OnGameStarted;
    public Action OnGameover; 
    
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += SceneLoaded; 

    }

    public void PauseGame() 
    {
        InputManager.Instance.inputActions.Player.Disable();
        Time.timeScale = 0f; 
        OnGamePaused?.Invoke();
    }
    public void ResumeGame() 
    {
        Time.timeScale = 1f; 
        OnGameStarted?.Invoke();
    }
    
    public void Gameover() {
        OnGameover?.Invoke();
        Time.timeScale = 0f;
        AudioManager.Instance.StopAudioByType(AudioType.SFX);
        AudioManager.Instance.StopAudioByType(AudioType.Music);
        AudioManager.Instance.StopAudioByType(AudioType.Ambience);
    }
    public void RestartGame() { 
        Time.timeScale = 1f;
        InputManager.Instance.inputActions.Player.Enable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        Time.timeScale = 1f;
        if(scene.name != "MainMenu")
            OnGameStarted?.Invoke();
    }

    
}

