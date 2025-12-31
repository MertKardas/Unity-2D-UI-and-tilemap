internal interface IInteractable {
    bool TryInteract(PlayerController playerController);
    bool CanInteract { get; }
    bool IsInRange { get; set; }
}