using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FoodSpawner : MonoBehaviour
{

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private GameObject currentFood;
    private bool isDraggingFood = false;

    [SerializeField]
    private ModeManager modeManager;

    [SerializeField] private Transform currentTransform;

    [SerializeField] private float throwForce = 0.005f;
    [SerializeField] private float upwardForce = 0.005f;
    [SerializeField] private float spawnDelay = 0.5f; // スポーンのクールダウン


    [Header("スポーンさせる食べ物")]
    [SerializeField] private GameObject[] foodPrefabs; // 複数のエサのプレハブを格納する配列
    private int foodObjectIndex = 0;



    private void OnEnable()
    {
        SpawnFood();
    }

    private void OnDisable()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
        }
        CancelInvoke(nameof(SpawnFood));
    }

    // エサを生成して手元にセットする処理
    private void SpawnFood()
    {
        // 既にエサがある場合は何もしない
        if (currentFood != null) return;

        currentFood = Instantiate(foodPrefabs[foodObjectIndex]);
        foodObjectIndex++;
        if (foodObjectIndex >= foodPrefabs.Length)
        {
            foodObjectIndex = 0;
        }

        currentFood.transform.SetParent(currentTransform);
        currentFood.transform.localPosition = Vector3.zero;
        currentFood.tag = "Untagged";// タグを "Untagged" に設定して、スライムが追いかけないようにする

        Rigidbody rb = currentFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 待機中は物理演算をオフ
        }
    }




    void Update()
    {
        if (modeManager == null || modeManager.currentMode != ModeManager.GameMode.FeedSlime) return;
        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if(currentFood == null) return;

                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == currentFood)
                    {
                        startTouchPosition = touch.screenPosition;
                        isDraggingFood = true;
                    }
                }
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended && isDraggingFood)
            {
                endTouchPosition = touch.screenPosition;
                ThrowFood(startTouchPosition, endTouchPosition);
                isDraggingFood = false;
            }
        }
    }
    private void ThrowFood(Vector2 start, Vector2 end)
    {
        Vector2 swipeVector = end - start;
        if (swipeVector.magnitude < 100f) // スワイプが短すぎる場合は無視
            return;

        Transform camTransform = Camera.main.transform;
        Rigidbody rb = currentFood.GetComponent<Rigidbody>();

        if (rb != null)
        {
            currentFood.transform.SetParent(null); // 親を外す
            currentFood.tag = "Food"; // タグを "Food" に変更
            rb.isKinematic = false;

            // 2Dのスワイプの力を、3Dのカメラの向きに合わせて変換
            // 前方向の力 ＋ 上方向の力 ＋ 左右の力
            Vector3 force = (camTransform.forward * swipeVector.magnitude * throwForce)
                          + (camTransform.up * swipeVector.y * upwardForce)
                          + (camTransform.right * swipeVector.x * throwForce);

            // 瞬間的な力（Impulse）として加える
            rb.AddForce(force, ForceMode.Impulse);
        }

        currentFood = null; // 投げた後は currentFood を null にする
        Invoke(nameof(SpawnFood), spawnDelay); // 一定時間後に新しいエサを生成
    }
}
