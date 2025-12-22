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
        InputManager.Instance.DisablePlayerInput();
        Time.timeScale = 0f; 
        OnGamePaused?.Invoke();
    }
    public void ResumeGame() 
    {
        InputManager.Instance.EnablePlayerInput();
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
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void SceneLoaded(Scene scene, LoadSceneMode mode) {
        InputManager.Instance.EnablePlayerInput();
        Time.timeScale = 1f;
        if(scene.name != "MainMenu")
            OnGameStarted?.Invoke();
    }

    
}

