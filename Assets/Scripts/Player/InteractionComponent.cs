using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class InteractionComponent : MonoBehaviour,IComponent    
{
    [ShowNonSerializedField] private List<IInteractable> interactables = new List<IInteractable>();
    private PlayerController playerController;
    void IComponent.Initialize(PlayerController controller) {
        playerController = controller;
      
    }


    public void InteractAction() {
        if (interactables.Count == 0) return;

        IInteractable closest = interactables
            .Where(a => a is MonoBehaviour mb && mb.transform != null && a.CanInteract)
            .OrderBy(a => Vector2.Distance(transform.position, ((MonoBehaviour)a).transform.position))
            .FirstOrDefault();

        if (closest != null) {
            closest.Interact(playerController);
        }
    }
    

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.SetInRange(true);
            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.SetInRange(false);
            interactables.Remove(interactable);
        }
    }
}
