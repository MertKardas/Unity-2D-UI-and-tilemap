using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    
    bool IInteractable.CanInteract {get=> _isInRange;}

   
    bool _isInRange;
    bool IInteractable.IsInRange { get => _isInRange; set => _isInRange = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool IInteractable.TryInteract(PlayerController playerController)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
