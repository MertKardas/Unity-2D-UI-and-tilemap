using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {
    
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
    public bool CanInteract {
        get { return IsInRange && !IsOpen; }
        set { }
    }
    private bool isOpen = false;
    public bool IsOpen {
        get { return isOpen; }
        set 
        { 
            isOpen = value;
            Debug.Log("Chest is now open: " + value);
            if (value) animator.SetTrigger(openAnimationParameter);
        }
    }
    public bool TryInteract() {
        if (CanInteract) {
            IsOpen = true;
            return true;
        }
        return false;
    }
}
