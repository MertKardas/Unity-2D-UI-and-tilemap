internal interface IInteractable {
    bool TryInteract();
    bool CanInteract { get; set; }
    bool IsInRange { get; set; }
}