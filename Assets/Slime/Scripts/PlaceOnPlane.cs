using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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

    [Header("Flexible Color Picker の割り当て")]
    public FlexibleColorPicker fcp; 
    public FlexibleColorPicker fcp_Bottom;


    [SerializeField]
    private ModeManager modeManager;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        // インスペクタで未割当ならシーン内から検索して自動設定
        if (modeManager == null)
        {
            modeManager = FindObjectsByType<ModeManager>()[0];
        }
    }

    void Update()
    {

        if (modeManager == null || modeManager.currentMode != ModeManager.GameMode.SpawnSlime) return;
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];

            // UI（カラーピッカーなど）を操作している時はスライムを出さない
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.touchId))
            {
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                if (arRaycastManager.Raycast(touch.screenPosition, hitResults, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hitResults[0].pose;

                    // 1. スライムを生成
                    GameObject newSlime = Instantiate(_spawnedObjects[_spawnedObjectIndex], hitPose.position, hitPose.rotation);

                    // 2. FCPで現在選択されている色を直接取得して、スライムに適用！
                    if (fcp != null)
                    {
                        ChangeSlimeColor(newSlime, fcp.color,fcp_Bottom.color);
                    }
                }
            }
        }
    }

    private void ChangeSlimeColor(GameObject slime, Color newColor,Color bottomColor)
    {
        Renderer[] renderers = slime.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            // Shader Graphのプロパティ名 "_BaseColor" を指定して色を変更
            r.material.SetColor("_BaseColor", newColor);

            // 下半分の色も同じ色に変えたい場合は以下のコメントアウトを外します
            r.material.SetColor("_BottomColor", bottomColor); 
        }
    }
}