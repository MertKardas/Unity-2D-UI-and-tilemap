using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement; 
public class StartGameButtonUI : MenuButtonUI
{
    [Scene, SerializeField] private int Scene;
    public string SaveName { get; set; } = "New Save"; 
    public override void ButtonOnClick() {
        base.ButtonOnClick();
        SceneManager.LoadScene(Scene); 
        Debug.Log($"game saved{SaveName}"); 
    }
}
