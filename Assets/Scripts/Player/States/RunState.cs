using UnityEngine;

public class RunState : State<PlayerController>
{
    private MovementComponent _movement;
    private PlayerVisualComponent _visual;
    private Rigidbody2D _rigidbody;
    private Vector2 _input;

    public RunState(PlayerController controller, StateMachine<PlayerController> machine)
        : base(controller, machine)
    {
        _movement = controller.MovementComponent;
        _rigidbody = controller.Rigidbody;
        _visual = controller.VisualComponent;
    }

    public override void Enter()
    {
        base.Enter();
        _visual.SetRunning(true);
    }

    public override void Update()
    {
        _visual.AnimationDirection(_input);
    }

    public override void FixedUpdate()
    {
        _movement.RunCharacter(_input);
    }

    public override void Exit()
    {
        _visual.SetRunning(false);
    }

    public override void HandleInput()
    {
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
        if (_input.magnitude < 0.1f) _input = Vector2.zero;
    }

    public override void CheckTransitions()
    {
        var speed = _rigidbody.linearVelocity.magnitude;
        var isRunning = InputManager.Instance.ReadInput<float>(InputType.Run) > 0.5f;

        // Input yok ve hız düşük - Idle'a geç
        if (speed < 0.1f && _input == Vector2.zero)
        {
            var state = _machine.GetState<IdleState>();
            _machine.SetState(state);
            return;
        }

        // Sprint bırakıldı ama hareket var - Move'a geç
        if (!isRunning && _input != Vector2.zero)
        {
            var state = _machine.GetState<MoveState>();
            _machine.SetState(state);
        }
    }
}
