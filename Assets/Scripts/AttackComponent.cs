using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackComponent : MonoBehaviour
{
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody2D;
    public event Action OnAttackStarted;


    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
        _rigidbody2D = _playerController.GetComponent<Rigidbody2D>();

        InputManager.Instance.Subscribe(InputType.Attack,OnAttackInput, InputActionPhase.Performed);
        _playerController.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnAttackInput(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        if (_playerController.State == PlayerState.Dead || _playerController.State == PlayerState.Attacking)
            return;
       _playerController.State = PlayerState.Attacking;
        OnAttackStarted.Invoke();
        var direction = _playerController.playerData.isFlip ? Vector2.left : Vector2.right;
        _rigidbody2D.AddForce(direction , ForceMode2D.Impulse);
        Debug.Log($"Attack input received by " +
            $"{_rigidbody2D.name}" +
            $"{_playerController.State}");
    }

    
    private void OnPlayerDeath()
    {
        InputManager.Instance.Unsubscribe(InputType.Attack, OnAttackInput, InputActionPhase.Performed);
        
    }

   
}
