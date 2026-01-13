using System;
using UnityEngine;
using NaughtyAttributes;
public class HealthComponent : MonoBehaviour, IComponent, IDamagable
{
    PlayerController _playerController;

    public int Health = 100; 
    public int MaxHealth = 100; 

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnTakingDamage;


    void IComponent.Initialize(PlayerController controller)
    {
        _playerController = controller;
       
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

