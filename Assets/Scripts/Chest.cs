using UnityEngine;
   
public class Chest : MonoBehaviour, IInteractable, ISavable {
    public int Gold { get; set; } = 100;
    string ISavable.UniqueId => GetComponent<SaveableEntity>().UniqueId; 

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
            playerController.InventoryComponent.AddCoin(goldReceived);
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
    public object CaptureState() {
        return new ChestSaveData {
            isOpen = this.isOpen
        };
    }
    public void RestoreState(object data) {
        if(data == null) return;
        if (data is ChestSaveData saveData) {
            this.isOpen = saveData.isOpen;
            if (isOpen){
                animator.Play("Chest_Open", -1, 1f); // Set to the end of the open animation    
               
               
            }
        }
    }
}
[System.Serializable]
public class ChestSaveData {
    public bool isOpen;
}