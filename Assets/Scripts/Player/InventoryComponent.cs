using System.Collections.Generic;
using System;
using UnityEngine;

public class InventoryComponent : MonoBehaviour, IComponent
{
    
    private List<string> items = new List<string>();
    public int Coin { get; private set; } = 0;  
    public event Action<int> OnCoinChanged;
    public event Action<int> OnCoinChangedAmount;
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
    public void LoadInventory(List<string> inventory) {
        items = inventory;
    }
    public List<string> GetInventory() {
        return items;
    }

}
