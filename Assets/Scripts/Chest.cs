using UnityEngine;
   
public class Chest : MonoBehaviour, IInteractable {
    public int Gold { get; set; } = 100;
    private bool isOpen = false;
    [SerializeField] AudioData openSound;
    public bool IsInRange {
        get => _isInRange;
        set {
            if (value) {
                animator.SetBool(chestInteractable, true);
                _isInRange = true;
            } else {
                animator.SetBool(chestInteractable, false);
                _isInRange = false;
            }

        }
    }
    private bool _isInRange = false;

    [SerializeField] private string openAnimationParameter = "isOpen";
    [SerializeField] private string chestInteractable = "isInteractable";
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    public bool CanInteract => IsInRange && !IsOpen;
       
    
    public bool IsOpen {
        get { return isOpen; }
        set 
        { 
            isOpen = value;
            Debug.Log("Chest is now open: " + value);
            if (value){
                animator.SetTrigger(openAnimationParameter);
                AudioManager.Instance.PlaySound(openSound);
                
            }
        }
    }
    public bool TryInteract(PlayerController playerController) {
      
        if (CanInteract) {
            int goldReceived = OpenChest();
            //playerController.Coin += OpenChest();
            return true; 
        }
        else {
            return false;
        }
    }
    public int OpenChest() {
        int gold = Gold;
        IsOpen = true;
        return gold;    
    }
}
