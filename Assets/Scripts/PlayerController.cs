using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float growthRate = 0.1f; 
    public float lookSensitivity = 1f;
    public float jumpForce = 20f;

    [Tooltip("CinemachineのFollowに設定するターゲット")]
    public Transform cinemachineCameraTarget; 

    private Rigidbody rb;
    private float moveX;
    private float moveZ;
    private float lookX;
    private float lookY;
    private Transform mainCameraTransform;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    [Header("スライムのJointオブジェクト")]
    [SerializeField] private Rigidbody[] _childRigidbodies;

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        Vector2 moveInput = context.ReadValue<Vector2>();
        moveX = moveInput.x;
        moveZ = moveInput.y;


        if (moveInput == Vector2.zero)
        {
            // 入力が無い時はSleepさせる
            for (int i = 0; i < _childRigidbodies.Length; i++)
            {
                if (_childRigidbodies[i] != null)
                {
                   // _childRigidbodies[i].Sleep();
                }
            }
        }
        else
        {
            // 入力がある（移動し始めた）時はWakeUpで起こす
            for (int i = 0; i < _childRigidbodies.Length; i++)
            {
                if (_childRigidbodies[i] != null)
                {
                    _childRigidbodies[i].WakeUp();
                }
            }
        }

    }

    public void OnLook(InputAction.CallbackContext context)
    {

        Vector2 lookInput = context.ReadValue<Vector2>();
        lookX = lookInput.x;
        lookY = lookInput.y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0f; 
        cameraForward = cameraForward.normalized;

        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            //rb.MoveRotation(targetRotation);
            //Debug.Log(rb.rotation.eulerAngles);
        }

        Vector3 moveDirection = transform.forward * moveZ + transform.right * moveX;
        moveDirection = moveDirection.normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
    }

    void LateUpdate() 
    {

        if (cinemachineCameraTarget == null) return;

        _cinemachineTargetYaw += lookX * lookSensitivity;
        _cinemachineTargetPitch -= lookY * lookSensitivity;

        _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, -89f, 89f);

        cinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            transform.localScale += new Vector3(growthRate, growthRate, growthRate);
        }
    }
}