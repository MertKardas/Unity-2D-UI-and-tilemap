using UnityEngine; 
public class IdleState : State<PlayerController> {
    private Vector2 _input;

    private PlayerVisualComponent Visual => base._controller.VisualComponent;
    public IdleState(PlayerController controller, StateMachine<PlayerController> machine)
        : base(controller, machine) { }

    public override void Enter() {
        base.Enter();
        Visual.SetMoving(false);

        var lastDirection = Visual.GetLastDirection();
        Visual.AnimationDirection(lastDirection);
    }

    public override void HandleInput() {
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
    }

    public override void CheckTransitions() {
        // Hareket input'u varsa Move'a geç
        if (_input != Vector2.zero) {
            var state = _machine.GetState<MoveState>();
            _machine.SetState(state);
        }
    }
}