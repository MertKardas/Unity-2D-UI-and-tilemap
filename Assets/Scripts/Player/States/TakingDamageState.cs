using System;
using UnityEngine;

public class TakingDamageState : State<PlayerController> {
    Rigidbody2D _rb; 
    MovementComponent _movementComponent;

    PlayerVisualComponent _visual; 
    public TakingDamageState(PlayerController controller, StateMachine<PlayerController> machine) 
        : base(controller, machine)
    {
        _movementComponent = controller.MovementComponent;
        _rb =controller.Rigidbody;
        _visual = controller.VisualComponent;
    }

    public override void Enter()
    {
        base.Enter();
        _visual.TriggerTakeDamage();
        _visual.OnFinishTakeDamage += OnFinishTakeDamage;
    }

    

    public override void Update()
    {
        base.Update();
        _movementComponent.MoveCharacter(Vector2.zero);

    }

    public override void Exit()
    {
        base.Exit();
        _visual.OnFinishTakeDamage -= OnFinishTakeDamage;
    }
    private void OnFinishTakeDamage() {
        var state = _machine.GetState<IdleState>();
        _machine.SetState(state);   
    }

}
