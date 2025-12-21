using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerAnimation : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]private AudioData footstepClip;
    [SerializeField] private AudioData takeDamageClip;
    [SerializeField] private AudioData deathClip;
    [SerializeField]private AudioData attackClip;


    private Animator _animator;
    private PlayerController _playerController;
    const string IsMoving = "IsMoving";
    const string isAttack = "isAttacking";
    private SpriteRenderer _spriteRenderer;
    public bool IsFlip { 
        get => _spriteRenderer.flipX;
        private set { 
            _spriteRenderer.flipX = value;
            _playerController.playerData.isFlip= value;
            }
        }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController.OnTakeDamage += (takenDamage) =>
        {
            _animator.SetTrigger("TakeDamage");
            AudioManager.Instance.PlaySound(takeDamageClip);    
        };
        _playerController.OnPlayerDeath += () =>
        {
            _animator.SetTrigger("Die");
            AudioManager.Instance.PlaySound(deathClip);
        };
        _playerController.AttackComponent.OnAttackStarted += TriggerAttackAnimation;
        _playerController.OnStateChanged += (previousState, newState) =>
        {
            if (newState == PlayerState.Attacking)
            {
                AttackSound();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimation();
        HandleFlip();
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
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
    private void HandleFlip()
    {
        if (_playerController == null) return;

        Vector2 movement = _playerController.Movement;

        if (movement.x > 0)
        {
            IsFlip = false;
        }
        else if (movement.x < 0)
        {
            IsFlip = true;
        }
    }
    public void PlayFootstepSound()
    {
        if (footstepClip == null) return;
        AudioManager.Instance.PlaySound(footstepClip);
    }
    public void AttackSound()
    {
        if (attackClip == null) return;
        AudioManager.Instance.PlaySound(attackClip);
    }

  
    public void TriggerAttackAnimation()
    {
        if (_animator != null)
        {
            _animator.SetBool(isAttack, true);
        }
    }
}
