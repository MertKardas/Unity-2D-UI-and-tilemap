using System;
using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public int Health {
        get { return (int)playerData.health; }
        set 
        { 
            playerData.health = value;
            if (playerData.health <= 0) {
                playerData.health = 0;
                OnPlayerDeath?.Invoke();
            } else { OnHealthChanged?.Invoke(); }
        }
    }
    public int Coin {
        get { return (int)playerData.coin; }
        set 
        { 
            playerData.coin = value;
            OnCoinChanged?.Invoke();
        }
    }
    public event Action OnHealthChanged;
    public event Action OnCoinChanged;
    public event Action OnTakeDamage; 
    public event Action OnPlayerDeath;
    public Vector2 Movement { get; private set; }
    Rigidbody2D _rb;
    Collider2D _collider2D;

    
    void OnEnable()
    {
        _collider2D = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        OnPlayerDeath += () => {
            InputManager.Instance.inputActions.Player.Disable();
        };

        InputManager.Instance.inputActions.Player.Enable();
        InputManager.Instance.inputActions.Player.Move.performed += ctx => {
            Movement = ctx.ReadValue<Vector2>() * playerData.speed;
        };
        InputManager.Instance.inputActions.Player.Move.canceled += ctx => {
            Movement = Vector2.zero;
        };
        
    }
    void OnDisable()
    {
        Movement = Vector2.zero;
        _rb.linearVelocity = Vector2.zero;
        InputManager.Instance.inputActions.Player.Disable();
        _collider2D.enabled = false;
    }



    // Update is called once per frame
    void Update()
    {
      
        _rb.linearVelocity = Movement;
        
       
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.TryGetComponent<Trap>(out Trap trap))
        {

            Debug.Log("Player in trap area");
            // Handle trap interaction
           
           
            Health -= trap.TakeDamage();
            OnTakeDamage?.Invoke();
           
            
        }
    }
}
[System.Serializable]
public class PlayerData {
   public PlayerState state = PlayerState.Idle;
   public float speed;
   public int health;
   public int coin;
   public float damage;
}
public enum PlayerState {
    Idle,
    Moving,
    TakeDamage,
    Interacting,
    Dead
}