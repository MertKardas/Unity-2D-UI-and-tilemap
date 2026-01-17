using UnityEngine;
using Newtonsoft.Json;
using System;
using NaughtyAttributes;

public class DropObject : MonoBehaviour, ICollectable, ISavable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _quantity = 1;

    string ISavable.UniqueId => GetComponent<SaveableEntity>().UniqueId;

    private void Start()
    {
        if (_quantity <= 0 || _itemData == null)
        {
            Debug.LogWarning($"DropObject {gameObject.name} has invalid data.");
            gameObject.SetActive(false);
        }
    }

    public InventoryItem Collect(PlayerController controller)
    {
        if (_itemData == null)
        {
            Debug.LogWarning("ItemData is null on DropObject.");
            return null;
        }
        
        if (_quantity <= 0)
        {
            Debug.LogWarning("DropObject has no quantity left to collect.");
            return null;
        }
        
        return new InventoryItem(_itemData.ID, _quantity);
    }

    public void OnItemsCollected(int amount)
    {
        if (amount <= 0) return;

        _quantity = Mathf.Max(_quantity - amount, 0);

        if (_quantity <= 0)
        {
            // Collider'ı devre dışı bırak ki tekrar tetiklenmessin
            GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
        }
    }

    object ISavable.CaptureState()
    {
        return new DropObjectSaveData
        {
            itemID = _itemData != null ? _itemData.ID : string.Empty,
            quantity = _quantity
        };
    }

    void ISavable.RestoreState(object data)
    {
        if (data is DropObjectSaveData saveData)
        {
            _quantity = saveData.quantity;
            
            if (!string.IsNullOrEmpty(saveData.itemID))
            {
                _itemData = ItemDataBase.Instance.GetItem(saveData.itemID);
                
                if (_itemData == null)
                {
                    Debug.LogError($"Could not find ItemData with ID: {saveData.itemID}");
                }
            }

            if (_quantity <= 0)
            {
                GetComponent<Collider2D>().enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}

[Serializable]
public class DropObjectSaveData
{
    public string itemID;
    public int quantity;
}