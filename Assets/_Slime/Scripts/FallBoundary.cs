using UnityEngine;

public class FallBoundary : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("チェックを入れるとスライム用の復帰処理、外すとエサ用の削除処理になります")]
    public bool isSlime = false; 
    
    [Tooltip("カメラの高さから何メートル下を落下と判定するか")]
    public float fallDistance = 1.5f;

    private Transform mainCamera;
    private Rigidbody rb;

    void Start()
    {
        mainCamera = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // カメラがない場合は処理しない
        if (mainCamera == null) return;

        // 奈落のボーダーラインを計算（スマホの高さ - 1.5mなど）
        float fallThreshold = mainCamera.position.y - fallDistance;

        // もし自分の高さがボーダーラインを下回ったら
        if (transform.position.y < fallThreshold)
        {
            if (isSlime)
            {
                //RespawnSlime();
                Destroy(gameObject);
            }
            else
            {
                // エサの場合は単純に削除する
                Destroy(gameObject);
            }
        }
    }

    void RespawnSlime()
        {
            // カメラの少し前、少し上空を復帰位置とする
            Vector3 respawnPos = mainCamera.position + mainCamera.forward * 0.5f;
            respawnPos.y += 0.2f;

            // 【重要】自分自身と、子オブジェクトについている全てのRigidbodyを取得
            Rigidbody[] allRbs = GetComponentsInChildren<Rigidbody>();

            // 1. ワープ前に、全てのRigidbodyの物理演算を一時停止し、勢いを殺す
            foreach (Rigidbody r in allRbs)
            {
                r.isKinematic = true;          // 物理演算をオフ
                r.linearVelocity = Vector3.zero;     // 落下や移動のスピードをリセット
                r.angularVelocity = Vector3.zero; // 回転の勢いをリセット
            }

            // 2. 物理演算がオフの状態で、安全に位置を移動させる
            transform.position = respawnPos;

            // 3. 移動が完了したら、再び全ての物理演算をオンに戻す
            foreach (Rigidbody r in allRbs)
            {
                r.isKinematic = false;
            }
        }
}