using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using UnityEngine.Rendering;
public class DropObject : MonoBehaviour,ICollactable {
    [SerializeField,JsonIgnore] private DropItemData item;
    public string itemId { get; private set; }

    private void Awake() {
        itemId = item.id;
        
    }
   
    public bool Collect(PlayerController controller, out string itemId) {
        itemId = this.itemId;
        // Implement your collection logic here
        return true;
    }
    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent<PlayerController>(out var player)) {
            return;
        }
        //Collect(this);
    }
}
interface ICollactable {
    public bool Collect(PlayerController controller, out string itemId);
}