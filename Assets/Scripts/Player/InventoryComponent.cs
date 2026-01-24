using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyAttributes;
using Newtonsoft.Json;
using System.IO;

public class InventoryComponent : MonoBehaviour, IComponent
{
    [SerializeField] private int _slotNumber = 5; // how many slots the inventory has
    public int SlotNumber => _slotNumber;
    private List<InventoryItem> items = new List<InventoryItem>();
    [field: SerializeField] public int Coin { get; private set; } = 0;
    public event Action<int> OnCoinChanged;
    public event Action<int> OnCoinChangedAmount;
    public event Action<List<InventoryItem>> OnInventoryChanged;
    public void Initialize(PlayerController controller) { }
    #region COin 
    public void AddCoin(int amount)
    {
        Coin += amount;
        OnCoinChanged?.Invoke(Coin);
        OnCoinChangedAmount?.Invoke(amount);
    }
    public void SetCoin(int amount)
    {
        Coin = amount;
        OnCoinChanged?.Invoke(Coin);
    }
    public void SpendMoney(int amount)
    {
        if (Coin >= amount)
        {
            Coin = Math.Max(Coin - amount, 0);
            OnCoinChanged?.Invoke(Coin);
            OnCoinChangedAmount?.Invoke(-amount);
        }
        else
        {
            Debug.LogWarning("Not enough coins to spend.");
        }
    }
    #endregion
    public void LoadInventory(List<InventoryItem> inventory)
    {
        items = inventory;
    }

    public List<InventoryItem> GetInventory()
    {
        return items;
    }

    /// <summary>
    /// Adds an item to the inventory. 
    /// </summary>
    /// <param name="inventoryItem"></param>
    /// <returns>Returns the number of items actually added.</returns>
    public int AddItem(InventoryItem inventoryItem)
    {
        var itemId = inventoryItem.item;
        var itemsToAdd = inventoryItem.quantity;
        int itemsAdded = 0;

        if (inventoryItem.itemData.IsStackable)
        {
            var stackSize = inventoryItem.itemData.StackSize;
            var existingItems = items.FindAll(i => i.item == itemId);

            // First, try to fill existing stacks
            foreach (var invItem in existingItems)
            {
                if (itemsToAdd <= 0) break;

                int availableSpace = stackSize - invItem.quantity;
                if (availableSpace > 0)
                {
                    int amountToAdd = Mathf.Min(availableSpace, itemsToAdd);
                    invItem.quantity += amountToAdd;
                    itemsAdded += amountToAdd;
                    itemsToAdd -= amountToAdd;
                }
            }

            // Then, create new stacks for remaining items
            while (itemsToAdd > 0)
            {
                if (items.Count >= _slotNumber)
                {
                    break; // Inventory full
                }

                int amountForNewStack = Mathf.Min(stackSize, itemsToAdd);
                items.Add(new InventoryItem(itemId, amountForNewStack));
                itemsAdded += amountForNewStack;
                itemsToAdd -= amountForNewStack;
            }
        }
        else
        {
            // Non-stackable items - each item takes one slot
            while (itemsToAdd > 0)
            {
                if (items.Count >= _slotNumber)
                {
                    break; // Inventory full
                }
                items.Add(new InventoryItem(itemId, 1));
                itemsAdded++;
                itemsToAdd--;
            }
        }

        if (itemsAdded > 0)
        {
            OnInventoryChanged?.Invoke(items);
        }

        return itemsAdded;
    }
    public bool HasItem(string itemId, int quantity)
    {
        int totalQuantity = 0;
        foreach (var item in items)
        {
            if (item.item == itemId)
            {
                totalQuantity += item.quantity;
                if (totalQuantity >= quantity)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool RemoveItem(string itemId, int quantity)
    {
        if (!HasItem(itemId, quantity))
        {
            return false;
        }

        int remaining = quantity;
        for (int i = items.Count - 1; i >= 0 && remaining > 0; i--)
        {
            if (items[i].item == itemId)
            {
                if (items[i].quantity > remaining)
                {
                    items[i].quantity -= remaining;
                    remaining = 0;
                }
                else
                {
                    remaining -= items[i].quantity;
                    items.RemoveAt(i);
                }
            }
        }

        OnInventoryChanged?.Invoke(items);
        return true;
    }

}
[Serializable]
public class InventoryItem
{
    [JsonIgnore]
    public ItemData itemData { get; private set; }
    public string item;
    public int quantity;
    [JsonConstructor]
    public InventoryItem(string item, int quantity)
    {
        this.item = item;
        itemData = ItemDataBase.Instance.GetItem(item);
        if (itemData == null)
        {
            Debug.LogError($"Failed to load ItemData for item ID: {item}");
        }
        this.quantity = quantity;
    }
    public InventoryItem(ItemData itemData, int quantity)
    {
        this.itemData = itemData;
        this.item = itemData.ID;
        this.quantity = quantity;
    }

}