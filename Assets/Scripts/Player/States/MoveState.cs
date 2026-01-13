using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : State<PlayerController>
{
    private MovementComponent _movement;
    private PlayerVisualComponent _visual;
    private Rigidbody2D _rigidbody;
    private Vector2 _input;

    public MoveState(PlayerController controller, StateMachine<PlayerController> machine) : base(controller, machine)
    {
        _movement = controller.MovementComponent;
        _rigidbody = controller.Rigidbody;
        _visual = controller.VisualComponent;
    }

    public override void Enter()
    {
        _visual.SetMoving(true);
       
    }

   

    public override void Update()
    {
        _visual.AnimationDirection(_input);
      
    }

    public override void FixedUpdate()
    {
        _movement.MoveCharacter(_input); 
    }

    public override void Exit()
    {
        _visual.SetMoving(false);
    }

    public override void CheckTransitions()
    {
        var speed = _rigidbody.linearVelocity.magnitude;
        //There is no input and speed is almost zero
        if (speed < 0.1f && _input == Vector2.zero)
        {
            var state = _machine.GetState<IdleState>();
            _machine.SetState(state);
        }
    }
    public override void HandleInput()
    {
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
        //Deadzone
        if (_input.magnitude <0.1f) _input = Vector2.zero;
    }
    
}
