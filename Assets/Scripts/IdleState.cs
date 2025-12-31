using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem; 

public class IdleState : State<PlayerController> {
    MovementComponent _movement;
    Rigidbody2D _rigidbody;
    PlayerVisualComponent _visual;
    InteractionComponent _interaction;
    HealthComponent _health;
    Vector2 _input;

    public IdleState(PlayerController controller, StateMachine<PlayerController> machine) : base(controller, machine) {
        _movement = controller.MovementComponent; 
        _rigidbody = controller.Rigidbody;
        _visual = controller.VisualComponent;
        _interaction = controller.InteractionComponent; 
        _health = controller.HealthComponent;
    }

    public override void Enter() {
        base.Enter();
        _visual.SetMoving(false);
        InputManager.Instance.Subscribe(InputType.Interact, OnInteract);
        _health.OnTakingDamage += OntakingDamage;
        var lastDirection = _visual.GetLastDirection();
        _visual.AnimationDirection(lastDirection);
    }
    public override void Update() {
        base.Update();
  
    }
    public override void FixedUpdate() {
        base.FixedUpdate();
       
    }

    public override void Exit()
    {
        base.Exit();
        InputManager.Instance.Unsubscribe(InputType.Interact, OnInteract);
        _health.OnTakingDamage -= OntakingDamage;
    }

    private void OntakingDamage() {
        var state = _machine.GetState<TakingDamageState>();
        _machine.SetState(state);
    }

    public override void InputHandle() {
        base.InputHandle();
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
        
    }
    public override void CheckTransitions() {
        base.CheckTransitions();
        if (_input != Vector2.zero) {
            var state = _machine.GetState<MoveState>();
            _machine.SetState(state);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interaction?.InteractAction();
        }
    }
}
