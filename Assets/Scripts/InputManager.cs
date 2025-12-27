using UnityEngine;
using MyUtility;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class InputManager : Singleton<InputManager> {
    private InputSystem_Actions _inputActions;
    private Dictionary<InputType, InputAction> _inputMap;
    
    protected override void Awake()
    {
        base.Awake();
        _inputActions = new InputSystem_Actions(); 
        _inputActions.Enable();
        _inputActions.UI.Enable();
        InitActionMap();


    }
    void InitActionMap() {
        _inputMap = new Dictionary<InputType, InputAction>{
            { InputType.Interact, _inputActions.Player.Interact },
            { InputType.Move, _inputActions.Player.Move },
            { InputType.Attack, _inputActions.Player.Attack },
            { InputType.Cancel, _inputActions.UI.Cancel },
        };
    }
    
    public T ReadInput<T>(InputType type) where T : struct {
        if (_inputMap.TryGetValue(type, out var action)) {
            return action.ReadValue<T>();
        }
        return default;
    }
    //Handle Interact Input
    public void Subscribe (InputType inputType,  
        Action<InputAction.CallbackContext> callback, 
         InputActionPhase phase = InputActionPhase.Performed) {
       if(callback == null || _inputActions == null)
           return;
        if (_inputMap.TryGetValue(inputType, out var action)) {
            switch (phase) {
                case InputActionPhase.Started:
                    action.started += callback;
                    break;
                case InputActionPhase.Performed:
                    action.performed += callback;
                    break;
                case InputActionPhase.Canceled:
                    action.canceled += callback;
                    break;
                case InputActionPhase.Disabled:
                    break;
            }
        }
    }

    public void Unsubscribe (InputType inputType,
       Action<InputAction.CallbackContext> callback,
        InputActionPhase phase = InputActionPhase.Performed) {
        if (callback == null || _inputActions == null)
            return;
        if (_inputMap.TryGetValue(inputType, out var action)) {
            switch (phase) {
                case InputActionPhase.Started:
                    action.started -= callback;
                    break;
                case InputActionPhase.Performed:
                    action.performed -= callback;
                    break;
                case InputActionPhase.Canceled:
                    action.canceled -= callback;
                    break;
                case InputActionPhase.Disabled:
                    break;
            }
        }
    }
    public void EnablePlayerInput()
    {
        _inputActions.Player.Enable();
    }   
    public void DisablePlayerInput()
    {
        _inputActions.Player.Disable();
    }
    public void EnableUIInput()
    {
        _inputActions.UI.Enable();
    }   
    public void DisableUIInput()
    {
        _inputActions.UI.Disable();
    }
    protected override void OnDestroy() {
        base.OnDestroy();
        _inputActions.Dispose();
    }
}
public enum InputType {
    //Player
    Interact,
    Move,
    Attack,
    //UI
    Cancel
}
