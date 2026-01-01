using NaughtyAttributes;
using System;
using UnityEngine;

public class InventoryComponent : MonoBehaviour, IComponent
{
    PlayerController _controller;
    PlayerRunTimeData _data;
    [ShowNativeProperty]public int Coin
        { get {return _data?.coin ?? 0; } 
        private set { _data.coin = value; } }
    public event Action<int> OnCoinChanged;
    public event Action<int> OnCoinChangedAmount;
    public void Initialize(PlayerController controller) {
        _controller = controller;
        _data = controller.playerData;

    }
    public void AddCoin(int amount) {
        Coin += amount;
        OnCoinChanged?.Invoke(Coin);
        OnCoinChangedAmount?.Invoke(amount);
    }
    

}
