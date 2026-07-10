using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class SlimeInteraction : MonoBehaviour
{
    [SerializeField]private ModeManager modeManager;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }


    void Update()
    {
        // エサやりモード（FeedSlime）の時だけ動作する
        if (modeManager == null || modeManager.currentMode != ModeManager.GameMode.FeedSlime) return;

        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];

            // UI操作中の誤爆を防止
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.touchId))
            {
                return;
            }

            // 画面に指が触れた瞬間（タップした瞬間）
            if (touch.phase == TouchPhase.Began)
            {
                // カメラからタップした位置に向かってレーザーを飛ばす
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                RaycastHit hit;

                // レーザーが何かのColliderにぶつかったら
                if (Physics.Raycast(ray, out hit))
                {
                    // ぶつかった相手が「Slime」タグを持っていたら
                    if (hit.collider.CompareTag("Player"))
                    {
                        // ぶつかったオブジェクトからSlimeAIを取得して喜ばせる
                        SlimeNPC slime = hit.collider.GetComponentInParent<SlimeNPC>();
                        if (slime != null)
                        {
                            slime.JoyReaction();
                        }
                    }
                }
            }
        }
    }
}