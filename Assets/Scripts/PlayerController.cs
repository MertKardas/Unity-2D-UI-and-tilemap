using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerRunTimeData playerData = new PlayerRunTimeData();
    
    public PlayerState State {
        get { return playerData.state; }
        set 
        { 
            if(playerData.state == value)
                return;
            PlayerState previousState = playerData.state;
            playerData.state = value;
            OnStateChanged?.Invoke(previousState, playerData.state);
        }
    }

    public int Health {
        get { return (int)playerData.health; }
        set 
        { 
            if(playerData.state == PlayerState.Dead)
                return;
            playerData.health = value;
            if (playerData.health <= 0 && playerData.state != PlayerState.Dead) {
                playerData.health = 0;
                playerData.state = PlayerState.Dead;
                OnPlayerDeath?.Invoke();
                OnHealthChanged?.Invoke(playerData.health);
                GameManager.Instance.Gameover();
            } else 
            { 
                OnHealthChanged?.Invoke(playerData.health); 
            }
        }
    }
    public int Coin {
        get { return (int)playerData.coin; }
        set 
        { 
            playerData.coin = value;
            OnCoinChanged?.Invoke(playerData.coin);
        }
    }
    public event Action<int> OnHealthChanged;
    public event Action<int> OnCoinChanged;
    public event Action<int> OnTakeDamage; 
    public event Action OnPlayerDeath;
    public event Action OnAttack;
    public event Action<PlayerState, PlayerState> OnStateChanged;
    public Vector2 Movement { get; private set; }
    Rigidbody2D _rb;
    Collider2D _collider2D;

    
    void OnEnable()
    {
        OnStateChanged += (previous, current) => { if(current == PlayerState.Dead) GameManager.Instance.Gameover(); };
        _collider2D = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        OnPlayerDeath += InputManager.Instance.DisablePlayerInput;
        
        InputManager.Instance.EnablePlayerInput(); 
        InputManager.Instance.inputActions.Player.Move.performed += OnMoveInput;
        InputManager.Instance.inputActions.Player.Move.canceled += OnMoveInputCanceled;
        InputManager.Instance.inputActions.Player.Attack.performed += ctx => OnAttack?.Invoke();
        OnAttack += Thrust;

    }
    void OnDisable()
    {
        if (InputManager.Instance == null || InputManager.Instance.inputActions == null)
            return;
        InputManager.Instance.inputActions.Player.Move.performed -= OnMoveInput;
        InputManager.Instance.inputActions.Player.Move.canceled -= OnMoveInputCanceled;
        InputManager.Instance.DisablePlayerInput();
    }



    // Update is called once per frame
    void FixedUpdate() {
        if ( _rb == null) return;
        if(State == PlayerState.Dead ) {
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        _rb.linearVelocity = Movement;
        
       
    }
    public void OnMoveInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        Movement = ctx.ReadValue<Vector2>() * playerData.speed;
    }
    private void OnMoveInputCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        Movement = Vector2.zero;
    }
    private void Thrust() {
        if (State != PlayerState.Dead) {
            _rb.AddForce(Vector2.right /2, ForceMode2D.Impulse);
          
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(playerData.state == PlayerState.Dead)
            return; 
        if (collision.TryGetComponent<Trap>(out Trap trap))
        {

            Debug.Log("Player in trap area");
            // Handle trap interaction
           
            int takenDamage = trap.InflictDamage(this);
            OnTakeDamage?.Invoke(takenDamage);
        }
    }
}
[System.Serializable]
public class PlayerRunTimeData {
   public PlayerState state = PlayerState.Idle;
   public bool IsFLip;
   public float speed;
   public int health;
   public int coin;
   public float damage;
   public List<ItemSO> items = new List<ItemSO>();
}
public enum PlayerState {
    Idle,
    Moving,
    TakeDamage,
    Interacting,
    Dead
}