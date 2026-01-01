using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State<PlayerController>
{
    private MovementComponent _movement;
    private PlayerVisualComponent _visual;
    private Rigidbody2D _rigidbody;
    private Vector2 _input;
    private float _velocityX;
    private InteractionComponent _interaction;
    private HealthComponent _health;
    public MoveState(PlayerController controller, StateMachine<PlayerController> machine) : base(controller, machine)
    {
        _movement = controller.MovementComponent;
        _rigidbody = controller.Rigidbody;
        _visual = controller.VisualComponent;
        _interaction = controller.InteractionComponent;
        _health = controller.HealthComponent;
    }

    public override void Enter()
    {
        base.Enter();
        _visual.SetMoving(true);
        InputManager.Instance.Subscribe(InputType.Interact, OnInteract);
        _health.OnTakingDamage += OntakingDamage; 
    }

    private void OntakingDamage() {
        var state = _machine.GetState<TakingDamageState>();
        _machine.SetState(state);
    }

    public override void Update()
    {
        base.Update();
        _visual.AnimationDirection(_input);

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _movement.MoveCharacter(_input); 
    }

    public override void Exit()
    {
        base.Exit();
        _visual.SetMoving(false);
        InputManager.Instance.Unsubscribe(InputType.Interact, OnInteract);
        _health.OnTakingDamage -= OntakingDamage;
    }

    public override void CheckTransitions()
    {
        base.CheckTransitions();
        var speed = _rigidbody.linearVelocity.magnitude;
        //There is no input and speed is almost zero
        if (speed < 0.1f && _input == Vector2.zero)
        {
            var state = _machine.GetState<IdleState>();
            _machine.SetState(state);
        }
    }
    public override void InputHandle()
    {
        base.InputHandle();
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
        //Deadzone
        if (_input.magnitude <0.1f) _input = Vector2.zero;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interaction?.InteractAction();
        }
    }
}
