using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected string _interactionPrompt = "Interact";

    protected bool _isInRange;

    public virtual bool CanInteract => _isInRange;
    public string InteractionPrompt => _interactionPrompt;

    public virtual void SetInRange(bool value)
    {
        _isInRange = value;
        OnRangeChanged(value);
    }

    /// <summary>
    /// Called when the player enters or exits interaction range.
    /// Override this to add custom behavior (e.g., highlight effects, UI prompts).
    /// </summary>
    protected virtual void OnRangeChanged(bool inRange) { }

    public abstract void Interact(PlayerController player);
}
