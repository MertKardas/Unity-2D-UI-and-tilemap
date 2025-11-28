using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerRunTimeData playerData = new PlayerRunTimeData();
    public int Health {
        get { return (int)playerData.health; }
        set 
        { 
            playerData.health = value;
            if (playerData.health <= 0) {
                playerData.health = 0;
                OnPlayerDeath?.Invoke();
            } else 
            { 
                OnHealthChanged?.Invoke(); 
            }
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
        OnPlayerDeath += InputManager.Instance.DisablePlayerInput;

        InputManager.Instance.EnablePlayerInput(); 
        InputManager.Instance.inputActions.Player.Move.performed += OnMoveInput;
        InputManager.Instance.inputActions.Player.Move.canceled += OnMoveInputCanceled;

    }
    void OnDisable()
    {
        
        Movement = Vector2.zero;
        _rb.linearVelocity = Vector2.zero; 
        _collider2D.enabled = false;
        if (InputManager.Instance == null || InputManager.Instance.inputActions == null)
            return;
        InputManager.Instance.inputActions.Player.Move.performed -= OnMoveInput;
        InputManager.Instance.inputActions.Player.Move.canceled -= OnMoveInputCanceled;
        InputManager.Instance.DisablePlayerInput();
    }



    // Update is called once per frame
    void Update()
    {
      
        _rb.linearVelocity = Movement;
        
       
    }
    public void OnMoveInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        Movement = ctx.ReadValue<Vector2>() * playerData.speed;
    }
    private void OnMoveInputCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        Movement = Vector2.zero;
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
public class PlayerRunTimeData {
   public PlayerState state = PlayerState.Idle;
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