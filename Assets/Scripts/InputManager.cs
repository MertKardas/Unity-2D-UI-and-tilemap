using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputSystem_Actions inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inputActions = new InputSystem_Actions();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}