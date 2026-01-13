using UnityEngine;

public class DeathState : State<PlayerController>
{
    PlayerVisualComponent _visual;
    public DeathState(PlayerController controller, StateMachine<PlayerController> machine) 
        : base(controller, machine)
    {
        _visual = controller.VisualComponent;
    }

    public override void Enter()
    {
        _controller.Rigidbody.linearVelocity = Vector2.zero;
        //Stop taking damage sound if any
        var audioData = _visual.takeDamageClip; 
        AudioManager.Instance.StopSound(audioData); 
        _visual.TriggerDeath();
        _visual.OnFinishDeathAnimation += GameManager.Instance.Gameover;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void HandleInput()
    {
    }

    public override void CheckTransitions()
    {
    }
}
