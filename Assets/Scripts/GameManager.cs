using UnityEngine;
using MyUtility;
using System;
public class GameManager : Singleton<GameManager>
{
    public Action OnGamePaused;
    public Action OnGameStarted;
    protected override void Awake()
    {
        base.Awake();
        
    }
    public void PauseGame() 
    {
        Time.timeScale = 0f; 
        OnGamePaused?.Invoke();
    }
    public void ResumeGame() 
    {
        Time.timeScale = 1f; 
        OnGameStarted?.Invoke();
    }

}

