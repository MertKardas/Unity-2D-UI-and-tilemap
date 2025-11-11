using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public Vector2 Movement { get; private set; }
    Rigidbody2D _rb;
    CapsuleCollider2D collider2D;
    void Awake()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<CapsuleCollider2D>();
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
    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.TryGetComponent<Trap>(out Trap trap))
        {
            Debug.Log("Player in trap area");
            // Handle trap interaction
            if (trap.CanInflictDamage)
            {
                playerData.health -= trap.Damage;
                Debug.Log($"Player took {trap.Damage} damage from trap. Current health: {playerData.health}");
            }
        }
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