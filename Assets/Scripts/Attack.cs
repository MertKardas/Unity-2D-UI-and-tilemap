using UnityEngine;

public class Attack : MonoBehaviour
{
    Animator _animator;
    PlayerRunTimeData _data;
    private void Init(Animator animator, PlayerRunTimeData data) {
        _animator = animator;
        _data = data;   
    }
    
}
