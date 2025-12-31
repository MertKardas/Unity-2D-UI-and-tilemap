using UnityEngine;
using NaughtyAttributes;
public class MovementComponent : MonoBehaviour, IComponent {
    
    Rigidbody2D _rb;
    PlayerController _playerController;

    [ShowNonSerializedField] private float _maxSpeed;
    [ShowNonSerializedField] private float _acceleration;
    [ShowNonSerializedField] private float _deceleration;

    public void Initialize(PlayerController controller) {
        _rb = controller.GetComponent<Rigidbody2D>();
        _playerController = controller;
        var data = controller.playerData;
        _maxSpeed = data.speed;
        _acceleration = data.acceleration;
        _deceleration = data.deceleration;
    }

    /// <summary>
    /// Move character based on input with acceleration and deceleration.
    /// </summary>
    public void MoveCharacter(Vector2 input) {
  
        Vector2 targetVelocity = input * _maxSpeed;
        Vector2 currentVelocity = _rb.linearVelocity;

        if (input.magnitude > 0.01f) {
           
            _rb.linearVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
        } else {
           
            _rb.linearVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }
    }
}
