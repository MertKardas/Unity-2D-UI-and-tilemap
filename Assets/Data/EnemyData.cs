using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject {

    public string EnemyName;
    public float MaxHealth;
    public float MaxSpeed;
    public int Damage;
    public float AttackRange;
    public float FollowRange;
    public float PatrolRadius;
}
