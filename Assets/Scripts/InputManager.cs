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
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
}