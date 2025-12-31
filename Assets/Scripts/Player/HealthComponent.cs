using System;
using UnityEngine;
using NaughtyAttributes;
public class HealthComponent : MonoBehaviour, IComponent, IDamagable
{
    PlayerController _playerController;
    PlayerRunTimeData data; 
    [ShowNativeProperty]public int Health {
        get {
            return data?.health ?? 0; }
        private set { data.health = value; }
    }
    [ShowNativeProperty] public int MaxHealth {
        get { return data?.maxHealth ?? 0; }
    }

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnTakingDamage;


    void IComponent.Initialize(PlayerController controller)
    {
        _playerController = controller;
        if(_playerController.playerData ==null) {
            Debug.LogError($"PlayerRunTimeData is not assigned in PlayerController.");
        }
        data = _playerController.playerData;
    }
    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;
        Health -= damage;
        Health = Mathf.Clamp(Health, 0, MaxHealth); 
        OnTakingDamage?.Invoke();
        OnHealthChanged?.Invoke(Health);
        if (Health <= 0)
        {
            OnDeath?.Invoke();
            var state = _playerController.Machine.GetState<DeathState>();
            _playerController.Machine.SetState(state);
        }
        
    }
   
}

