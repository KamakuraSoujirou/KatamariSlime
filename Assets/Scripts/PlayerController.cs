using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    [SerializeField] private float speed = 5f;
    [SerializeField] public float growthRate = 0.1f;
    private Rigidbody rb;

    private float moveX;
    private float moveZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext  value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        moveX = input.x;
        moveZ = input.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveX, 0.0f, moveZ) * speed;
        rb.linearVelocity = new Vector3(movement.x * speed, rb.linearVelocity.y, movement.z * speed);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            transform.localScale += Vector3.one * growthRate;
        }
    }
}
