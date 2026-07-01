using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
// 新しいInput SystemのEnhancedTouchを使用するための準備
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _spawnedObjects;
    private int _spawnedObjectIndex = 0;

    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    [Header("現在選択されている色")]
    public Color currentSlimeColor = Color.blue; // デフォルトの色

    // スクリプトが有効になった時にタッチ入力を検知開始する
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    // スクリプトが無効になった時にタッチ入力を検知終了する
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // 画面に指が触れているかチェック
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];

            // UIのボタンをタッチした時はスライムを出さない（誤爆防止）
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.touchId))
            {
                return;
            }

            // タッチした瞬間
            if (touch.phase == TouchPhase.Began)
            {
                // touch.screenPosition でタッチした画面の座標を取得
                if (arRaycastManager.Raycast(touch.screenPosition, hitResults, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hitResults[0].pose;

                    // 1. スライムを生成
                    GameObject newSlime = Instantiate(_spawnedObjects[_spawnedObjectIndex], hitPose.position, hitPose.rotation);

                    // 2. 生成したスライムの色を変更する
                    ChangeSlimeColor(newSlime, currentSlimeColor);
                }
            }
        }
    }

    // スライムの色を変更する処理
    private void ChangeSlimeColor(GameObject slime, Color newColor)
    {
        // スライム（子オブジェクト含む）のRendererを取得
        Renderer[] renderers = slime.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            // Shader Graphのプロパティ名 "_BaseColor" を指定して色を変更
            r.material.SetColor("_BaseColor", newColor);

            // 下半分の色も同じ色に変えたい場合は以下のコメントアウトを外します
            // r.material.SetColor("_BottomColor", newColor); 
        }
    }

    // ＝＝＝ 以下、UIボタンから呼び出すためのメソッド ＝＝＝
    public void SelectColorRed()
    {
        currentSlimeColor = Color.red;
    }

    public void SelectColorGreen()
    {
        currentSlimeColor = Color.green;
    }

    public void SelectColorBlue()
    {
        currentSlimeColor = Color.blue;
    }
}