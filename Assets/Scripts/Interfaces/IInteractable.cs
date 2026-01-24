public interface IInteractable
{
    bool CanInteract { get; }
    string InteractionPrompt { get; }
    void Interact(PlayerController player);
    void SetInRange(bool value);
}