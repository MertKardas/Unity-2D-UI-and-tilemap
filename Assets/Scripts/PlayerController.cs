using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public Vector2 Movement { get; private set; }
    Rigidbody2D _rb;

    void Awake()
    {
        
        _rb = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        InputManager.Instance.inputActions.Player.Move.performed += ctx => {
            Movement = ctx.ReadValue<Vector2>() * playerData.speed;
        };
        InputManager.Instance.inputActions.Player.Move.canceled += ctx => {
            Movement = Vector2.zero;
        };
        
    }
    void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
        _rb.linearVelocity = Movement;
        
       
    }
}
[System.Serializable]
public class PlayerData {
   public PlayerState state = PlayerState.Idle;
   public float speed;
   public int health;
   public int damage;
}
public enum PlayerState {
    Idle,
    Moving,
    Interacting,
    Dead
}