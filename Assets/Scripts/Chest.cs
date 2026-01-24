using UnityEngine;

public class Chest : InteractableBase, ISavable
{
    [SerializeField] private int _gold = 100;
    [SerializeField] private AudioData _openSound;
    [SerializeField] private string _openAnimationTrigger = "isOpen";
    [SerializeField] private string _interactableAnimationBool = "isInteractable";

    private Animator _animator;
    private bool _isOpen;

    public int Gold => _gold;
    public bool IsOpen => _isOpen;

    string ISavable.UniqueId => GetComponent<SaveableEntity>().UniqueId;

    public override bool CanInteract => base.CanInteract && !_isOpen;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected override void OnRangeChanged(bool inRange)
    {
        if (_animator != null)
        {
            _animator.SetBool(_interactableAnimationBool, inRange);
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!CanInteract) return;

        int goldReceived = OpenChest();
        player.InventoryComponent.AddCoin(goldReceived);
    }

    private int OpenChest()
    {
        _isOpen = true;

        if (_animator != null)
        {
            _animator.SetTrigger(_openAnimationTrigger);
        }

        if (_openSound != null)
        {
            AudioManager.Instance.PlaySound(_openSound);
        }

        return _gold;
    }

    public object CaptureState()
    {
        return new ChestSaveData { isOpen = _isOpen };
    }

    public void RestoreState(object data)
    {
        if (data is ChestSaveData saveData)
        {
            _isOpen = saveData.isOpen;

            if (_isOpen && _animator != null)
            {
                _animator.Play("Chest_Open", -1, 1f);
            }
        }
    }
}

[System.Serializable]
public class ChestSaveData
{
    public bool isOpen;
}
