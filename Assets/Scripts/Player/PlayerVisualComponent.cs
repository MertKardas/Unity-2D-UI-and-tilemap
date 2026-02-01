using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Audio; 

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualComponent : MonoBehaviour, IComponent {

    [Header("Audio Clips")]
    [SerializeField] public  AudioData footstepClip;
    [SerializeField] public  AudioData takeDamageClip;
    [SerializeField] public  AudioData deathClip;
    [SerializeField] public  AudioData attackClip;
    [SerializeField]public AudioResource test; 
    [SerializeField] public  float runMoveSpeedMultiplier = 1.5f;

    [Header("Animation Parameters")]
    public static readonly int MovinSpeedgHash = Animator.StringToHash("Move");
    public static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    public static readonly int MoveSpeedParamHash = Animator.StringToHash("MoveSpeed");
    public static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");
    public static readonly int DeathHash = Animator.StringToHash("Death");
    public static readonly int InputX = Animator.StringToHash("InputX");
    public static readonly int InputY = Animator.StringToHash("InputY");
    public Vector2 LastDirection = Vector2.down;

    // Events
    public event Action OnFinishTakeDamage;
    public event Action OnFinishDeathAnimation;
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;



    void IComponent.Initialize(PlayerController controller) {
        _animator = GetComponent<Animator>();
        if (_animator == null) {
            Debug.LogError("Animator component not found on PlayerVisualComponent!");
            return;
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer component not found on PlayerVisualComponent!");
            return;
        }
    }

    public void SetMoving(bool isMoving) {
        _animator.SetFloat(MovinSpeedgHash, isMoving ? 1f : 0f);
    }

    public void SetRunning(bool isRunning) {
        SetMoving(isRunning);
        _animator.SetFloat(MoveSpeedParamHash, isRunning ? runMoveSpeedMultiplier : 1f);
    }
    
    public void PlayFootstepSound() {
        AudioManager.Instance.PlaySound(footstepClip, transform.position);
    }

    public void AnimationDirection(Vector2 input) {
        Vector2 direction;

        if (input.magnitude < 0.1f) {
            // Hareket yoksa son y�n� kullan
            direction = LastDirection;
        } else {
            // Diagonal input kontrol�
            bool isDiagonal = Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) > 0.1f;

            if (isDiagonal) {
                // Son y�n�n hangi eksende oldu�una bak
                if (Mathf.Abs(LastDirection.x) > 0.1f) {
                    // Son y�n yataydaysa, yatay ekseni kullan
                    direction = new Vector2(Mathf.Sign(input.x), 0);
                } else {
                    // Son y�n dikeydeyse, dikey ekseni kullan
                    direction = new Vector2(0, Mathf.Sign(input.y));
                }
            } else {
                // Tek y�nl� input - normal 4 y�n kilidi
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                    direction = new Vector2(Mathf.Sign(input.x), 0);
                } else {
                    direction = new Vector2(0, Mathf.Sign(input.y));
                }
            }

            LastDirection = direction;
        }
        

        // Her iki parametreyi de set et
        _animator.SetFloat(InputX, direction.x);
        _animator.SetFloat(InputY, direction.y);
    }
    public Vector2 GetLastDirection() => LastDirection;
    
    public void TriggerTakeDamage() {
        _animator.SetTrigger(TakeDamageHash);
        AudioManager.Instance.PlaySound(takeDamageClip, transform.position);
    }
    public void TriggerDeath() {
        _animator.SetTrigger(DeathHash);
        AudioManager.Instance.PlaySound(deathClip, transform.position);
    }
    public void FinishTakingDamage() {
        OnFinishTakeDamage?.Invoke(); 
    }
    public void FinishDeathAnimation() {
        OnFinishDeathAnimation?.Invoke();
        _spriteRenderer.enabled = false;
    }
}

