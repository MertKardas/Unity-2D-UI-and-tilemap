using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyAttributes;

public class InventoryComponent : MonoBehaviour, IComponent
{
    
    private List<InventoryItem> items = new List<InventoryItem>();
    [field: SerializeField]public int Coin { get; private set; } = 0;  
    public event Action<int> OnCoinChanged;
    public event Action<int> OnCoinChangedAmount;
    public event Action<List<InventoryItem>> OnInventoryChanged;
    public void Initialize(PlayerController controller) {}
    public void AddCoin(int amount) {
        Coin += amount;
        OnCoinChanged?.Invoke(Coin);
        OnCoinChangedAmount?.Invoke(amount);
    }
    public void SpendMoney(int amount) {
        if (Coin >= amount) {
            Coin = Math.Max(Coin - amount, 0);
            OnCoinChanged?.Invoke(Coin);
            OnCoinChangedAmount?.Invoke(-amount);
        } else {
            Debug.LogWarning("Not enough coins to spend.");
        }
    }
    public void LoadInventory(List<InventoryItem> inventory) {
        items = inventory;
    }
    
    public List<InventoryItem> GetInventory() {
        return items;
    }
    public void AddItem(string itemId) {
        var itemData = ItemDataBase.Instance.GetItem(itemId);
        if(itemData == null) {
            Debug.LogWarning($"Item with ID {itemId} not found in database.");
            return;
        }
      
        if(itemData.IsStackable) {
            var stackSize = itemData.StackSize;
            var existingItems = items.FindAll(i => i.item == itemId);
            if(existingItems.Count > 0) {
                //check for available stack space
                foreach(var invItem in existingItems) {
                    if(invItem.quantity < stackSize) {
                        invItem.quantity += 1;
                        OnInventoryChanged?.Invoke(items);
                        return;
                    }
                }
                //All existing stacks are full, create a new stack
                items.Add(new InventoryItem(itemId, 1));   
                OnInventoryChanged?.Invoke(items);
                return; 
            }  
        }
        else {
            //Item is not stackable, add a new entry
            items.Add(new InventoryItem(itemId, 1));
            OnInventoryChanged?.Invoke(items);
            return;
        }
    }
}

[Serializable]
public class InventoryItem
{
    public string item;
    public int quantity;
    public InventoryItem(string item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

