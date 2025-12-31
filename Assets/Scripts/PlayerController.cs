
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Player data. 
    public PlayerRunTimeData playerData = new PlayerRunTimeData();

    [Header("Components")]
    public AttackComponent AttackComponent;
    public InteractionComponent InteractionComponent;
    public HealthComponent HealthComponent;
    public StateMachine<PlayerController> Machine;
    public MovementComponent MovementComponent; 
    public PlayerVisualComponent VisualComponent;
    public Rigidbody2D Rigidbody;


    /*
    public int Coin {
        get { return (int)playerData.coin; }
        set 
        { 
            playerData.coin = value;
            OnCoinChanged?.Invoke(playerData.coin);
        }
    }*/



    private void Awake() {
        //State Machine setup
        

        (AttackComponent as IComponent)?.Initialize(this);
        (InteractionComponent as IComponent)?.Initialize(this);
        (MovementComponent as IComponent)?.Initialize(this);
        (VisualComponent as IComponent)?.Initialize(this);
        (HealthComponent as IComponent)?.Initialize(this);
     
    
    }
    private void Update() {
        Machine?.Update();
    }
    private void FixedUpdate() {
        Machine?.FixedUpdate();
    }


    private void Start() {
        Machine = new StateMachine<PlayerController>();
        Machine.AddState(new IdleState(this, Machine));
        Machine.AddState(new MoveState(this, Machine));
        Machine.AddState(new TakingDamageState(this, Machine));
        Machine.AddState(new DeathState(this, Machine));
        Machine.SetState(Machine.GetState<IdleState>());
    }


}
//TODO runtime data is carried out later
[System.Serializable]
public class PlayerRunTimeData {
   public State<PlayerController> state;
   public bool isFlip;
    //Movement
    public float speed;
    public float acceleration;
    public float deceleration;
    //Health
    public int health;
   public int maxHealth;

   public int coin;
   public float damage;
   public List<ItemSO> items = new List<ItemSO>();
}
