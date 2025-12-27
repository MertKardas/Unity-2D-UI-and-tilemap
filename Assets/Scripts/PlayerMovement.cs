using UnityEngine; // Unity.VisualScripting sildim, gereksiz.

public class PlayerMovement : MonoBehaviour {
    Rigidbody2D _rb;
    PlayerController _playerController;

    Vector2 _input;

    // H�z ayarlar� (Bunlar� PlayerData'dan da �ekebilirsin)
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _acceleration = 50f; // H�zlanma ivmesi
    [SerializeField] private float _deceleration = 100f; // Durma ivmesi (Daha y�ksek olmal� ki karakter �abuk dursun)

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();

        // Null Check'ler gayet yerinde
        if (_playerController == null) Debug.LogError("PlayerController component not found!");
        if (_rb == null) Debug.LogError("Rigidbody2D component not found!");
    }

    private void Update() {
        if (_playerController.playerData.state == PlayerState.Dead) {
            _input = Vector2.zero; // �l�nce inputu s�f�rla
            return;
        }

        // Input'u al ama Normalize et!
        // Normalize etmezsen �apraz giderken (1,1) b�y�kl��� ~1.4 olur, karakter �aprazda daha h�zl� ko�ar.
        _input = InputManager.Instance.ReadInput<Vector2>(InputType.Move).normalized;
    }

    private void FixedUpdate() {
        if (_playerController.playerData.state == PlayerState.Dead) {
            // �l�yken fiziksel olarak da durdurmak istersen:
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        CalculateMovement();
    }

    private void CalculateMovement() {

        Vector2 targetVelocity = _input * _maxSpeed;


        Vector2 currentVelocity = _rb.linearVelocity;

        if (_input.magnitude > 0.01f) {
           
            _rb.linearVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
        } else {
           
            _rb.linearVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }
    }
}