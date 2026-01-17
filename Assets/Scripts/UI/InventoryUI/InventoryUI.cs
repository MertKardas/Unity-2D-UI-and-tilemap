using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private PlayerController _playerController;
    private List<InventoryItem> _inventoryItems; 
    private List<InventorySlotUI> _inventorySlotsUIs = new List<InventorySlotUI>();
    private int _maxSlotNumber; 

    void Start()
    {   
        if (_playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in InventoryUI");
            return;
        }
        
        _inventoryItems = _playerController.InventoryComponent.GetInventory();
        _maxSlotNumber = _playerController.InventoryComponent.SlotNumber;
        InitInventorySlots();
        UpdateInventory(_inventoryItems);
    }
    
    void OnEnable()
    {
        if (_playerController == null) 
        {
            Debug.LogWarning("PlayerController is null in OnEnable of InventoryUI");
            return; 
        }
        _playerController.InventoryComponent.OnInventoryChanged += UpdateInventory;
    }
    
    void OnDisable()
    {
        if (_playerController == null) 
        {
            Debug.LogWarning("PlayerController is null in OnDisable of InventoryUI");
            return;
        }
        
        _playerController.InventoryComponent.OnInventoryChanged -= UpdateInventory;
    }
    
    private void InitInventorySlots() 
    {
        for (int i = 0; i < _maxSlotNumber; i++) 
        {
            var slotUI = Instantiate(_inventorySlotPrefab, transform);
            var slotUIComponent = slotUI.GetComponent<InventorySlotUI>();
            slotUIComponent.Init(i);
            _inventorySlotsUIs.Add(slotUIComponent);
        }
    }
    
    public void UpdateInventory(List<InventoryItem> items) 
    {
        _inventoryItems = items;
        for (int i = 0; i < _inventorySlotsUIs.Count; i++) 
        {
            if (i < _inventoryItems.Count) 
            {
                _inventorySlotsUIs[i].UpdateSlot(_inventoryItems[i]);
            } 
            else 
            {
                _inventorySlotsUIs[i].UpdateSlot(null);
            }
        }
    }
}