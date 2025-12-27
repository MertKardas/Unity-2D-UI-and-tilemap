
using System;
using System.Collections.Generic;
using UnityEngine;

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
                OnHealthChanged?.Invoke(playerData.health);
                State = PlayerState.Dead;
                InputManager.Instance.DisablePlayerInput();
                 LeanTween.delayedCall(0.6f,
                    () => GameManager.Instance.Gameover());
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
    //public event Action OnPlayerDeath;

    public event Action<PlayerState, PlayerState> OnStateChanged;
    public Vector2 Movement { get; private set; }

    public AttackComponent AttackComponent;
    private void Awake() {
        AttackComponent = GetComponent<AttackComponent>();
        AttackComponent.Init(this);
    }
    
    

    private void OnTriggerEnter2D(Collider2D collision) {
        if(playerData.state == PlayerState.Dead)
            return; 
        if (collision.TryGetComponent<Trap>(out Trap trap))
        {

            Debug.Log("Player in trap area");
            // Handle trap interaction
           
            int takenDamage = trap.InflictDamage(this);
            
            State = PlayerState.TakeDamage;
            OnTakeDamage?.Invoke(takenDamage);
        }
    }
}
[System.Serializable]
public class PlayerRunTimeData {
   public PlayerState state = PlayerState.Idle;
   public bool isFlip;
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
    Dead,
    Attacking
}