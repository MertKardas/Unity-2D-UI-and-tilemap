using UnityEngine;
using NaughtyAttributes;
public class MovementComponent : MonoBehaviour, IComponent {
    Rigidbody2D rb;
    [field: SerializeField] public float MaxSpeed { get; set; }
    [field: SerializeField] public float RunSpeed { get; set; }
    [field: SerializeField] public Vector2 CurrentVelocity { get; set; }
    [field: SerializeField] public float Acceleration { get; set; }
    [field: SerializeField] public float Deceleration { get; set; }
    public void Initialize(PlayerController controller) {
        rb = controller.Rigidbody;
    }
 

    /// <summary>
    /// Move character based on input with acceleration and deceleration.
    /// </summary>
    public void MoveCharacter(Vector2 input) {
  
        Vector2 targetVelocity = input * MaxSpeed;
        Vector2 currentVelocity = rb.linearVelocity;

        if (input.magnitude > 0.01f) {

            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, Acceleration * Time.fixedDeltaTime);
            CurrentVelocity = rb.linearVelocity;
        } else {

            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, Deceleration * Time.fixedDeltaTime);
            CurrentVelocity = rb.linearVelocity;
        }
    }

    /// <summary>
    /// Run character based on input with acceleration and deceleration.
    /// </summary>
    public void RunCharacter(Vector2 input) {
        Vector2 targetVelocity = input * RunSpeed;
        Vector2 currentVelocity = rb.linearVelocity;

        if (input.magnitude > 0.01f) {
            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, Acceleration * Time.fixedDeltaTime);
            CurrentVelocity = rb.linearVelocity;
        } else {
            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, Deceleration * Time.fixedDeltaTime);
            CurrentVelocity = rb.linearVelocity;
        }
    }
}
