using UnityEngine;

public class TakeDamageBehaviour : StateMachineBehaviour
{
    //Change state when the animation ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController playerController = animator.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.State != PlayerState.Dead)
                playerController.State = PlayerState.Idle;
        }
    }
}
