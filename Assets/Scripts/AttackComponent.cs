using UnityEngine;
using UnityEngine.InputSystem;
public class AttackComponent : MonoBehaviour
{
    PlayerController _playerController;
    Rigidbody2D rigidbody2D;
    
    public void Init(PlayerController playerController) {
        _playerController = playerController;
        InputManager.Instance.inputActions.Player.Attack.performed += OnAttackInput;
        rigidbody2D = _playerController.GetComponent<Rigidbody2D>();
    }
    public void OnAttackInput(InputAction.CallbackContext ctx) {
        if (_playerController.State == PlayerState.Dead || _playerController.State == PlayerState.Attacking) return;
        if (ctx.performed) {
            _playerController.State = PlayerState.Attacking;
            rigidbody2D.AddForce((_playerController.playerData.IsFLip ? Vector2.left : Vector2.right), ForceMode2D.Impulse);
            Debug.Log("Attack performed");
            Invoke(nameof(FinishAttack), 0.5f);
        }
    }
    public void FinishAttack() {
        if (_playerController.State == PlayerState.Dead) return;
        _playerController.State = PlayerState.Idle;
    }
}
