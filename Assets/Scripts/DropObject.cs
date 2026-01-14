using UnityEngine;
using Newtonsoft.Json;
using System;
using NaughtyAttributes;

public class DropObject : MonoBehaviour,ICollectable, ISavable {
   
    [SerializeField]private ItemData _itemData;

    public bool IsCollected { get; private set; } = false;
    string ISavable.UniqueId => GetComponent<SaveableEntity>().UniqueId;

   
    private void Start() {
        if (IsCollected) {
            gameObject.SetActive(false);
        }
       
    }

    public ItemData Collect(PlayerController controller) {
        if (IsCollected) {
            return null; 
        }
        gameObject.SetActive(false);
        IsCollected = true;
        return this._itemData;
    }
    

    object ISavable.CaptureState() {
        return new DropObjectSaveData {
            isCollected = this.IsCollected
        };
    }

    void ISavable.RestoreState(object data) {
        if(data == null) return;
        if (data is DropObjectSaveData saveData) {
            this.IsCollected = saveData.isCollected;
        }
    }
}
interface ICollectable {
    public ItemData Collect(PlayerController controller);
}
[Serializable]
public class DropObjectSaveData {  
    public bool isCollected;
}