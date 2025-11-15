using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    NavMeshAgent agent; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //2D NavMeshAgent setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, playerTarget.position) < 3f)
        {
            agent.SetDestination(playerTarget.position);
        }
    }
}
