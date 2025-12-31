using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackComponent : MonoBehaviour,IComponent
{
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody2D;



    void IComponent.Initialize(PlayerController playerController)
    {
        _playerController = playerController;
        _rigidbody2D = _playerController.GetComponent<Rigidbody2D>();

    }

    public void OnAttackInput(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        var direction = _playerController.playerData.isFlip ? Vector2.left : Vector2.right;
        _rigidbody2D.AddForce(direction , ForceMode2D.Impulse);
       
    }

}
