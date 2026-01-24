using System.Buffers.Text;
using UnityEngine;
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

        //test save for now 
        

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
    public override void Update() {
        if(Keyboard.current.f5Key.IsPressed()) {
            Debug.Log("F5 pressed - Saving game.");
            SaveManager.Instance.QuickSave();
        }
        base.Update();
    }

    private void OnTakeDamage() {
        // Parent state'e ge�i�
        var state = _machine.GetState<TakingDamageState>();
        _machine.SetState(state);
    }

    private void OnDeath() {
        // Parent state'e ge�i�
        var state = _machine.GetState<DeathState>();
        _machine.SetState(state);
    }

    private void OnInteract(InputAction.CallbackContext context) {
        if (context.performed) {
            _interaction?.InteractAction();
        }
    }
    private void HandleSaveButton(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("F5 pressed - Saving game.");
            SaveManager.Instance.QuickSave();
        }
    }
}