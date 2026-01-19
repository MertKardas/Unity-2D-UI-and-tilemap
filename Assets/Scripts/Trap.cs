using DentedPixel;
using UnityEngine;

public class Trap : MonoBehaviour, ISavable
{
    [SerializeField] int Damage = 10;
    int _waitingAnimationHashed; 
    int _activeTrapHashed;
    [SerializeField] private AudioData trapSFX;
    [SerializeField] private Vector2 randomTrapActivate = new Vector2(1f, 2f);
    bool _isactive = false; 
    float _waitingTimer; 
    private float _animationNormalizedTime;

    private Animator animator;
    private BoxCollider2D trapCollider;

    string ISavable.UniqueId => GetComponent<SaveableEntity>().UniqueId;

    private void Awake()
    {
        trapCollider = GetComponent<BoxCollider2D>();
         animator = GetComponent<Animator>();
        _waitingAnimationHashed = Animator.StringToHash("Trap_Waiting"); 
        _activeTrapHashed = Animator.StringToHash("Trap_Activate");
        trapCollider.enabled = false;
       
       
    }
    void Update()
    {
        if(_isactive)
        {
            //Trap is active, do nothing
            return;
        }
        else
        {
            _waitingTimer -= Time.deltaTime;
            if(_waitingTimer <= 0f)
            {
                ActivateTrap();
            }
        }
    }
    public void ActivateTrap(float animationNormalizedTime = 0f)
    {
        _isactive = true;
        animator.Play(_activeTrapHashed, 0, animationNormalizedTime);
    }
    //Activated by animation event
    public void ActivatePeaks()
    {
     
        AudioManager.Instance.PlaySound(trapSFX);
        trapCollider.enabled = true;
    }
    //Deactivated by animation event
    public void DeactivatePeaks()
    {

        trapCollider.enabled = false;
    }
    //called by animation event
    public void InflictDamage(IDamageable damagable) {

        damagable.TakeDamage(Damage); 
    }
    //Activated first time onEnable
    
    private void OnTriggerEnter2D(Collider2D collision) {
        // Check if the colliding object has the IDamagable interface
        if (collision.TryGetComponent<IDamageable>(out var damagable))
        {
            InflictDamage(damagable);
        }
    }

    object ISavable.CaptureState()
    {
        return new TrapSaveData
        {
            isActive = _isactive,
            animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime
        };
    }

    void ISavable.RestoreState(object data)
    {
        if(data == null)
        {//SetDefaultState
            _isactive = false;
            _waitingTimer = Random.Range(randomTrapActivate.x, randomTrapActivate.y);
            return;
        }  
        else
        {
            var saveData = (TrapSaveData)data;
            _isactive = saveData.isActive;
            if(!_isactive)
            {
                _waitingTimer = Random.Range(randomTrapActivate.x, randomTrapActivate.y);
            }else
            {
                ActivateTrap(saveData.animationTime);
            }
        }
        
    }

}


