using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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
    
    private void OnEnable() {

        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_playerController == null) return;   
            _playerController.OnStateChanged += OnStateChange;
        if( _playerController.AttackComponent != null) 
            _playerController.AttackComponent.OnAttackStarted += TriggerAttackAnimation;
     
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
        if(_playerController.playerData.state == PlayerState.Dead) return;
        Vector2 movement = InputManager.Instance.ReadInput<Vector2>(InputType.Move);

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
        if(_playerController.playerData.state == PlayerState.Dead) return;
        Vector2 movement = InputManager.Instance.ReadInput<Vector2>(InputType.Move);

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
    private void OnStateChange(PlayerState previous, PlayerState current)
    {
        Debug.Log($"Player state changed from {previous} to {current} in PlayerAnimation");
        if (current == PlayerState.Dead) {
            _animator.SetTrigger("Die");
            AudioManager.Instance.PlaySound(deathClip);
        }else if (current == PlayerState.TakeDamage) {
            Debug.Log("Player took damage, triggering animation.");
            _animator.SetTrigger("TakeDamage");
            AudioManager.Instance.PlaySound(takeDamageClip);
        } else if (current == PlayerState.Attacking) {
            AttackSound();
        }
    }
   
    private void OnDisable() {
        _playerController.OnStateChanged -= OnStateChange;
        _playerController.AttackComponent.OnAttackStarted -= TriggerAttackAnimation;
    }
}
