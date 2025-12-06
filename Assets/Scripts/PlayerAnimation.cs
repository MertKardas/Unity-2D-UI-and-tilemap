using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerAnimation : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]private AudioClip footstepClip;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField]private AudioClip attackClip;

    private AudioSource _audioSource; 
    private Animator _animator;
    private PlayerController _playerController;
    const string IsMoving = "IsMoving";
    const string isAttack = "isAttacking";
    private SpriteRenderer _spriteRenderer;
    public bool IsFlip { 
        get => _spriteRenderer.flipX;
        private set { 
            _spriteRenderer.flipX = value;
            _playerController.playerData.IsFLip= value;
            }
        }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _playerController.OnTakeDamage += (takenDamage) =>
        {
            _animator.SetTrigger("TakeDamage");
            _audioSource.PlayOneShot(takeDamageClip);
           
        };
        _playerController.OnPlayerDeath += () =>
        {
            _animator.SetTrigger("Die");
            _audioSource.PlayOneShot(deathClip);
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
        if (_audioSource != null && footstepClip != null)
        {
            _audioSource.pitch = Random.Range(0.8f, 1.2f);
            _audioSource.volume = Random.Range(0.4f, 0.7f) * AudioManager.Instance.Volume;
            _audioSource.PlayOneShot(footstepClip);
        }
    }
    public void AttackSound()
    {
        if (_audioSource == null || attackClip == null) return;
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.volume = Random.Range(0.4f, 0.7f) * AudioManager.Instance.Volume;
        _audioSource.PlayOneShot(attackClip);
    }

  
    public void TriggerAttackAnimation()
    {
        if (_animator != null)
        {
            _animator.SetBool(isAttack, true);
        }
    }
}
