using UnityEngine;
using MyUtility; 
public class InputManager : Singleton<InputManager> {
    public InputSystem_Actions inputActions;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
        inputActions.UI.Enable();
    }
    public void EnablePlayerInput()
    {
        inputActions.Player.Enable();
    }   
}