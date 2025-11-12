internal interface IInteractable {
    bool TryInteract(PlayerController playerController);
    bool CanInteract { get; set; }
    bool IsInRange { get; set; }
}