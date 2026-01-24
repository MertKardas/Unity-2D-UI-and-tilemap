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

    private bool _isLocked;

    public override bool CanInteract => base.CanInteract && !_isLocked;
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
        }
        else
        {
            if(_lockedDoorAudio != null)
                AudioManager.Instance.PlaySound3D(_lockedDoorAudio, transform.position);
        }
    }
    public override void SetInRange(bool value)
    {
        base.SetInRange(value);
        _highlightObject.SetActive(value);
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
        if (data is DoorSaveData saveData)
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
