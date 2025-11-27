using NaughtyAttributes;
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class EnemyController : MonoBehaviour {
    [Header("Enemy Data")]
    public EnemyData enemyData;
    public EnemySaveData saveData = new EnemySaveData();

    [Header("Component References")]
    private NavMeshAgent _agent;
    private BehaviorGraphAgent _graph;
    EnemyMovement _enemyMovement; 

    public int CurrentHealth {
        get { return saveData.CurrentHealth; }
        set {
            saveData.CurrentHealth = value;
            if (saveData.CurrentHealth <= 0) {

            }
        }
    }
    
    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _graph = GetComponent<BehaviorGraphAgent>();
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.Init(enemyData, _agent);

       
    }
    private void GraphSetup() {
        
        _graph.BlackboardReference.SetVariableValue<float>("FollowRange", enemyData.FollowRange);
        _graph.BlackboardReference.SetVariableValue<float>("AttackRange", enemyData.AttackRange);
    }


}
[Serializable]
public class EnemySaveData {
    public Vector3 Position;
    public int CurrentHealth;
   
}





