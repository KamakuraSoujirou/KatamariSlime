using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float growthRate = 0.1f; 
    public float lookSensitivity = 1f;

    // ★追加：Cinemachineに追いかけさせるターゲット
    [Tooltip("CinemachineのFollowに設定するターゲット（空のオブジェクト等）を割り当ててください")]
    public Transform cinemachineCameraTarget; 

    private Rigidbody rb;
    private float moveX;
    private float moveZ;
    private float lookX;
    private float lookY;
    private Transform mainCameraTransform;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // 修正：ReadValueに変更
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveX = moveInput.x;
        moveZ = moveInput.y;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // 修正：ReadValueに変更
        Vector2 lookInput = context.ReadValue<Vector2>();
        lookX = lookInput.x;
        lookY = lookInput.y;
    }

    void FixedUpdate()
    {
        // 移動処理は変更なし（Main Cameraの向いている方向を基準に移動する）
        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0f; 
        cameraForward = cameraForward.normalized;

        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            rb.MoveRotation(targetRotation);
        }

        Vector3 moveDirection = transform.forward * moveZ + transform.right * moveX;
        moveDirection = moveDirection.normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
    }

    void LateUpdate() 
    {
        // ターゲットが設定されていなければ処理しない
        if (cinemachineCameraTarget == null) return;

        _cinemachineTargetYaw += lookX * lookSensitivity;
        _cinemachineTargetPitch -= lookY * lookSensitivity;

        _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, -89f, 89f);

        // ★変更：Main Cameraではなく、ターゲットオブジェクトを回転させる
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