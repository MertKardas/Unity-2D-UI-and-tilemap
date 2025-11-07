using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    InputSystem_Actions inputActions;
    Vector2 Movement; 
    Rigidbody2D _rb;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        _rb = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Movement = inputActions.Player.Move.ReadValue<Vector2>();
        _rb.linearVelocity = Movement;
        Debug.Log(Movement);
    }
}
