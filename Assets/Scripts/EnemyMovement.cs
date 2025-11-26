using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    NavMeshAgent _agent;
    EnemyData _enemyData;
    Rigidbody2D _rb;
    public void Init(EnemyData enemyData, NavMeshAgent agent) {
        _agent = agent;
        _enemyData = enemyData;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = _enemyData.MaxSpeed;
        _agent.acceleration = 1000f;
        _agent.angularSpeed = 0; 
    }
    public void MoveToPosition(Vector3 targetPosition) {
        if (_agent == null) return;

        _agent.isStopped = false;
        _agent.SetDestination(new Vector3(targetPosition.x, targetPosition.y, 0));
    }
    public void StopMovement() {
        if (_agent == null) return;
        _agent.isStopped = true;
    }

    // Hedefe ulaştı mı?
    public bool HasReachedDestination() {
        if (_agent == null) return false;

        return !_agent.pathPending
            && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}