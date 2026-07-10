using UnityEngine;
using System.Collections; // コルーチンを使うために必要です

public class FloatInSlime : MonoBehaviour
{
    [Header("浮遊の設定")]
    // 浮遊する振幅（どれくらい上下に動くか）
    public float amplitude = 0.1f;
    // 浮遊するスピード
    public float floatSpeed = 2f;
    // 回転するスピード
    public Vector3 rotateSpeed = new Vector3(15f, 30f, 45f);

    [Header("消化（消滅）の設定")]
    // 小さくなり始めるまでの待機時間（秒）
    public float timeBeforeShrink = 2f;
    // 小さくなって完全に消えるまでの時間（秒）
    public float shrinkDuration = 1.5f;

    private Vector3 startLocalPosition;
    private float randomOffset;

    private TrailRenderer trailRenderer;

    void Start()
    {
        // 吸収された時点での、スライム体内での初期位置（ローカル座標）を記憶
        startLocalPosition = transform.localPosition;

        // 全てのアイテムが同じタイミングで同じ動きをすると不自然なので、タイミングをランダムにずらす
        randomOffset = Random.Range(0f, 100f);

        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false; // TrailRendererを無効化
        }

        StartCoroutine(ShrinkAndDestroyRoutine());
    }

    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime);

        float newY = startLocalPosition.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * amplitude;

        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);
    }


    private IEnumerator ShrinkAndDestroyRoutine()
    {
        // 1. 指定した時間（timeBeforeShrink）だけ、そのままのサイズで待機する
        yield return new WaitForSeconds(timeBeforeShrink);

        float time = 0;
        Vector3 initialScale = transform.localScale;

        // 2. 指定した時間（shrinkDuration）かけて徐々に小さくする
        while (time < shrinkDuration)
        {
            time += Time.deltaTime;
            float t = time / shrinkDuration;

            // 現在のサイズ(initialScale)から、ゼロ(Vector3.zero)に向けて滑らかに縮小
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            yield return null; // 1フレーム待つ
        }

        // 3. 最後に念のため完全にサイズをゼロにする
        transform.localScale = Vector3.zero;

        // 4. オブジェクトを破壊して消去（消化完了）
        Destroy(gameObject);
    }
}