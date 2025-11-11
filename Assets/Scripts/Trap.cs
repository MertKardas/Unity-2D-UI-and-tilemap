using UnityEngine;

public class Trap : MonoBehaviour
{
    public int Damage = 10;

    public bool CanInflictDamage = false;

    private void Awake()
    {

    }
    //Activated by animation event
    public void ActivateTrap()
    {
        CanInflictDamage = true;
    }
    //Deactivated by animation event
    public void DeactivateTrap()
    {
        CanInflictDamage = false;
    }
    
}
