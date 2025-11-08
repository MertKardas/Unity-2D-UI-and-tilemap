using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;
    const string IsMoving = "IsMoving";
    private SpriteRenderer _spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimation();
        HandleDirection();  
    }

    private void HandleAnimation()
    {
        if (_playerController == null) return;

        Vector2 movement = _playerController.Movement;

        if (movement != Vector2.zero)
        {
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    private void HandleDirection()
    {
        if (_playerController == null) return;

        Vector2 movement = _playerController.Movement;

        if (movement.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
    }
}
