using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class InventorySlotUI : MonoBehaviour
{
    private int m_index;
    public int Index => m_index;
    [SerializeField] Image _icon = null;
    [SerializeField] TextMeshProUGUI _itemNumberText;
    
    public void Init(int index)
    {
        if (_icon == null || _itemNumberText == null)
        {
            Debug.LogError("Icon or ItemNumberText is not assigned in InventorySlotUI");
        }
       
        m_index = index;
        _icon.enabled = false;
        _icon.sprite = null;
        _itemNumberText.text = "";
    }
    
    public void UpdateSlot(InventoryItem item)
    {
        if (item != null && item.itemData != null)
        {
            _icon.enabled = true;
            _icon.sprite = item.itemData.Icon;
            _itemNumberText.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }
        else
        {
            _icon.sprite = null;
            _icon.enabled = false;
            _itemNumberText.text = "";
        }
    }
}