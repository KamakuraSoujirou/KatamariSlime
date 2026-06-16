using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public float throwForce = 0.005f;
    public float upwardForce = 0.005f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    void Start()
    {
        EnhancedTouchSupport.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startTouchPosition = touch.screenPosition;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                endTouchPosition = touch.screenPosition;
                ThrowFood(startTouchPosition, endTouchPosition);
            }
        }
    }
    private void ThrowFood(Vector2 start, Vector2 end)
    {
        Vector2 swipeVector = end - start;
        if (swipeVector.magnitude < 50f) // スワイプが短すぎる場合は無視
            return;
        Transform camTransform = Camera.main.transform;
        Vector3 spawnPos = camTransform.position + camTransform.forward * 0.3f;
        GameObject food = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 2Dのスワイプの力を、3Dのカメラの向きに合わせて変換
            // 前方向の力 ＋ 上方向の力 ＋ 左右の力
            Vector3 force = (camTransform.forward * swipeVector.magnitude * throwForce)
                          + (camTransform.up * swipeVector.y * upwardForce)
                          + (camTransform.right * swipeVector.x * throwForce);

            // 瞬間的な力（Impulse）として加える
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
