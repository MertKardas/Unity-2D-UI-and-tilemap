using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] int Damage = 10;
    [SerializeField] private AudioData trapSFX;
    [SerializeField] private Vector2 randomTrapActivate = new Vector2(1f, 2f);

    private Animator animator;
    private BoxCollider2D trapCollider;
    private void Awake()
    {
        trapCollider = GetComponent<BoxCollider2D>();
        trapCollider.enabled = false;
        animator = GetComponent<Animator>();
       
    }
    private void OnEnable() {
        float randomTime = Random.Range(randomTrapActivate.x, randomTrapActivate.y);
        Invoke(nameof(ActivateTrapFirstTme), randomTime);
    }
    //Activated by animation event
    public void ActivateTrap()
    {
     
        AudioManager.Instance.PlaySound(trapSFX);
        trapCollider.enabled = true;
    }
    //Deactivated by animation event
    public void DeactivateTrap()
    {

        trapCollider.enabled = false;
    }
    //called by animation event
    public void InflictDamage(IDamageable damagable) {

        damagable.TakeDamage(Damage); 
    }
    //Activated first time onEnable
    private void ActivateTrapFirstTme()
    {
        animator.SetBool("isTrapActive", true);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        // Check if the colliding object has the IDamagable interface
        if (collision.TryGetComponent<IDamageable>(out var damagable))
        {
            InflictDamage(damagable);
        }
    }

}
