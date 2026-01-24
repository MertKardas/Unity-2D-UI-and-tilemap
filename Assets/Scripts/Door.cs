using System;
using NaughtyAttributes;
using UnityEngine;

public class Door : InteractableBase, ISavable
{

    [Header("Door Audio")]
    [SerializeField] private AudioData _lockedDoorAudio;
    [SerializeField] private AudioData _unlockDoorAudio;
    [SerializeField] private AudioData _openDoorAudio;
    [SerializeField] private GameObject _highlightObject;
    [SerializeField] private ItemData _keyItem;
    private string _keyItemId => _keyItem != null ? _keyItem.ID : string.Empty;

    private bool _isLocked = true;

    public override bool CanInteract => base.CanInteract;
    public string UniqueId => GetComponent<SaveableEntity>().UniqueId;

    public override void Interact(PlayerController player)
    {
        if (!_isLocked)
        {
            if(_openDoorAudio != null)
                AudioManager.Instance.PlaySound3D(_openDoorAudio, transform.position);
            GameManager.Instance.EndGame();
            return;
        }

        // Kapı kilitli - anahtar kontrolü yap
        if (player.InventoryComponent.HasItem(_keyItemId, 1))
        {
            if(_unlockDoorAudio != null)
                AudioManager.Instance.PlaySound3D(_unlockDoorAudio, transform.position);
            Unlock();
            player.InventoryComponent.RemoveItem(_keyItemId, 1);
        }
        else
        {
            if(_lockedDoorAudio != null)
                AudioManager.Instance.PlaySound3D(_lockedDoorAudio, transform.position);
        }
    }
    protected override void OnRangeChanged(bool inRange)   
    {
        _highlightObject.SetActive(inRange);
    }
    public void Unlock() => _isLocked = false;
    public void Lock() => _isLocked = true;

    public object CaptureState()
    {
        return new DoorSaveData
        {
            IsLocked = _isLocked
        };
    }

    public void RestoreState(object data)
    {
        if (data is DoorSaveData saveData && saveData != null)
        {
            _isLocked = saveData.IsLocked;
        }
    }
}

[Serializable]
public class DoorSaveData
{
    public bool IsLocked;
}
