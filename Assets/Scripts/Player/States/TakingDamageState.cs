using NUnit.Framework;
using System;
using UnityEngine;

public class TakingDamageState : State<PlayerController> {
    
    MovementComponent _movementComponent;
    PlayerVisualComponent _visual; 
    public TakingDamageState(PlayerController controller, StateMachine<PlayerController> machine) 
        : base(controller, machine)
    {
        _movementComponent = controller.MovementComponent;
        _visual = controller.VisualComponent;
    }

    public override void Enter()
    {
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
        _visual.OnFinishTakeDamage -= OnFinishTakeDamage;
    }
    private void OnFinishTakeDamage() {
        var state = _machine.GetState<LocomotionState>();
        _machine.SetState(state);   
    }

}
