using NaughtyAttributes;
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, ISavable
{
    [Header("Enemy Data")]
    [Expandable]
    [SerializeField] private EnemyData _enemyData;

    [Header("Components")]
    private NavMeshAgent _agent;
    private BehaviorGraphAgent _graph;
    private EnemyMovement _enemyMovement;

    [Header("Runtime")]
    [ShowNonSerializedField] private int _currentHealth;

    public int CurrentHealth => _currentHealth;
    public bool IsDead => _currentHealth <= 0;
    public string UniqueId => GetComponent<SaveableEntity>().UniqueId;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _graph = GetComponent<BehaviorGraphAgent>();
        _enemyMovement = GetComponent<EnemyMovement>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _enemyMovement.Init(_enemyData, _agent);
        _currentHealth = (int)_enemyData.MaxHealth;

        GraphSetup();
    }

    private void GraphSetup()
    {
        _graph.BlackboardReference.SetVariableValue("FollowRange", _enemyData.FollowRange);
        _graph.BlackboardReference.SetVariableValue("AttackRange", _enemyData.AttackRange);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        _currentHealth -= damage;

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        _enemyMovement.StopMovement();
        _graph.End();
        // Add death logic: animation, loot, destroy, etc.
    }

    public object CaptureState()
    {
        return new EnemySaveData
        {
            Position = transform.position,
            CurrentHealth = _currentHealth
        };
    }

    public void RestoreState(object data)
    {
        if (data is EnemySaveData saveData)
        {
            transform.position = saveData.Position;
            _currentHealth = saveData.CurrentHealth;

            if (IsDead)
            {
                Die();
            }
        }
    }
}

[Serializable]
public class EnemySaveData
{
    public Vector3 Position;
    public int CurrentHealth;
}





