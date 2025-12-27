using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class Interact : MonoBehaviour
{
    private List<IInteractable> interactables = new List<IInteractable>();
    private PlayerController playerController;
    private void Start() {
        playerController = GetComponentInParent<PlayerController>();
        InputManager.Instance.Subscribe(InputType.Interact, InteractAction, InputActionPhase.Performed);
        playerController.OnStateChanged += OnStateChange;
    }
    private void InteractAction(InputAction.CallbackContext ctx) {
        if (!ctx.performed) return; 
        Debug.Log("Interact pressed");
        if (interactables.Count == 0) return;
        IInteractable closest = interactables
        .Where(a => a is MonoBehaviour mb && mb.transform != null) // Filter valid objects
        .OrderBy(a => Vector2.Distance(transform.position, ((MonoBehaviour)a).transform.position))
        .FirstOrDefault();

        if (closest != null) {
            bool success = closest.TryInteract(playerController);
            if (success)
                interactables.Remove(closest);

        }
    }
    public void OnStateChange(PlayerState previous, PlayerState current) {
        if(current == PlayerState.Dead) {
            interactables.Clear();
            InputManager.Instance.Unsubscribe(InputType.Interact, InteractAction, InputActionPhase.Performed);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.IsInRange = true;
            if (interactable.CanInteract) {
                interactables.Add(interactable);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.IsInRange = false;
            interactables.Remove(interactable);
        }
    }
}
