using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] int Damage = 10;
    [SerializeField] private AudioClip trapSFX; 
    private BoxCollider2D trapCollider;
    private void Awake()
    {
        trapCollider = GetComponent<BoxCollider2D>();
    }
    //Activated by animation event
    public void ActivateTrap()
    {
     
        SoundManager.Instance.PlaySound(trapSFX);
        trapCollider.enabled = true;
    }
    //Deactivated by animation event
    public void DeactivateTrap()
    {

        trapCollider.enabled = false;
    }
    public int TakeDamage() {
       
        return Damage; 
    
    }
    
}
