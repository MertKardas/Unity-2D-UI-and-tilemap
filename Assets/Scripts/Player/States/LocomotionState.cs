using UnityEngine.InputSystem;

public class LocomotionState : HierarchicalState<PlayerController> {
    private HealthComponent _health;
    private InteractionComponent _interaction;

    public LocomotionState(PlayerController controller, StateMachine<PlayerController> machine)
        : base(controller, machine) {
        _health = controller.HealthComponent;
        _interaction = controller.InteractionComponent;

        // Sub-state'leri ekle
        AddSubState(new IdleState(controller, _subStateMachine));
        AddSubState(new MoveState(controller, _subStateMachine));
    }

    public override void Enter() {
        // Ortak event subscriptions
        _health.OnTakingDamage += OnTakeDamage;
        _health.OnDeath += OnDeath;
        InputManager.Instance.Subscribe(InputType.Interact, OnInteract);

        // Default sub-state'i baþlat
        if (_subStateMachine.CurrentState == null) {
            var idleState = _subStateMachine.GetState<IdleState>();
            _subStateMachine.SetState(idleState);
        }
        base.Enter();
    }

    public override void Exit() {
        base.Exit();

        // Unsubscribe events
        _health.OnTakingDamage -= OnTakeDamage;
        _health.OnDeath -= OnDeath;
        InputManager.Instance.Unsubscribe(InputType.Interact, OnInteract);
    }

    private void OnTakeDamage() {
        // Parent state'e geçiþ
        var state = _machine.GetState<TakingDamageState>();
        _machine.SetState(state);
    }

    private void OnDeath() {
        // Parent state'e geçiþ
        var state = _machine.GetState<DeathState>();
        _machine.SetState(state);
    }

    private void OnInteract(InputAction.CallbackContext context) {
        if (context.performed) {
            _interaction?.InteractAction();
        }
    }
}