
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles player movements and interactions within the game.
/// </summary>
[RequireComponent(typeof(SaveableEntity))]
public class PlayerController : MonoBehaviour, ISavable
{
    //Player data. 


    [Header("Components")]
    public AttackComponent AttackComponent;
    public InteractionComponent InteractionComponent;
    public HealthComponent HealthComponent;
    public StateMachine<PlayerController> Machine;
    public MovementComponent MovementComponent;
    public PlayerVisualComponent VisualComponent;
    public InventoryComponent InventoryComponent;
    public Rigidbody2D Rigidbody;

    public string UniqueId => GetComponent<SaveableEntity>().UniqueId;

    private void Awake()
    {
        Machine = new StateMachine<PlayerController>();
        Machine.AddState(new LocomotionState(this, Machine));
        Machine.AddState(new TakingDamageState(this, Machine));
        Machine.AddState(new DeathState(this, Machine));
        InitializeComponents();
        //Default state

    }
    private void Update()
    {
        Machine?.Update();
    }
    private void FixedUpdate()
    {
        Machine?.FixedUpdate();


    }


    private void Start()
    {
        if (Machine.CurrentState == null)
        {
            Machine.SetState(Machine.GetState<LocomotionState>());
        }

    }
    private void InitializeComponents()
    {
        (AttackComponent as IComponent)?.Initialize(this);
        (InteractionComponent as IComponent)?.Initialize(this);
        (MovementComponent as IComponent)?.Initialize(this);
        (VisualComponent as IComponent)?.Initialize(this);
        (HealthComponent as IComponent)?.Initialize(this);
        (InventoryComponent as IComponent)?.Initialize(this);
    }
    public object CaptureState()
    {
        return new PlayerSaveData
        {
            StateData = Machine.GetCurrentStateData(),
            position = new float[] { transform.position.x, transform.position.y },
            velocity = new float[] { MovementComponent.CurrentVelocity.x, MovementComponent.CurrentVelocity.y },
            acceleration = MovementComponent.Acceleration,
            deceleration = MovementComponent.Deceleration,
            MaxSpeed = MovementComponent.MaxSpeed,
            health = HealthComponent.Health,
            maxHealth = HealthComponent.MaxHealth,
            coin = InventoryComponent.Coin,
            items = InventoryComponent.GetInventory()
        };
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ICollectable>(out var collectable))
        {
            var item = collectable.Collect(this);
            if (item != null)
            {
                int itemsAdded = InventoryComponent.AddItem(item);

                if (collectable is DropObject dropObject)
                {
                    dropObject.OnItemsCollected(itemsAdded);
                }
            }
        }
    }

    public void RestoreState(object data)
    {
        if (data is PlayerSaveData saveData)
        {

            transform.position = new Vector3(saveData.position[0], saveData.position[1], 0);
            MovementComponent.CurrentVelocity = new Vector2(saveData.velocity[0], saveData.velocity[1]);
            MovementComponent.Acceleration = saveData.acceleration;
            MovementComponent.Deceleration = saveData.deceleration;
            MovementComponent.MaxSpeed = saveData.MaxSpeed;
            HealthComponent.Health = saveData.health;
            HealthComponent.MaxHealth = saveData.maxHealth;
            InventoryComponent.LoadInventory(saveData.items);
            InventoryComponent.SetCoin(saveData.coin);
            // Restore player state from saveData
            if (saveData.StateData != null)
            {
                Machine.RestoreState(saveData.StateData);

            }
        }

    }

    
}
