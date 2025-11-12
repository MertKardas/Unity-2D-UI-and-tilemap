using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private List<IInteractable> interactables = new List<IInteractable>();
    private PlayerController playerController;
    private void Start() {
        playerController = GetComponentInParent<PlayerController>();
        InputManager.Instance.inputActions.Player.Interact.performed += ctx => {
            Debug.Log("Interact pressed");
            if (interactables.Count == 0) return;
            IInteractable? closest = interactables
            .Where(a => a is MonoBehaviour mb && mb.transform != null) // Filter valid objects
            .OrderBy(a => Vector2.Distance(transform.position, ((MonoBehaviour)a).transform.position))
            .FirstOrDefault();

            if (closest != null) {
                bool success = closest.TryInteract(playerController);
                if(success)
                    interactables.Remove(closest);

            }
        };
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
