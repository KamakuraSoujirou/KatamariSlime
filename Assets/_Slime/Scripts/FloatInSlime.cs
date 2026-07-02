using UnityEngine;

public class FloatInSlime : MonoBehaviour
{
    // 浮遊する振幅（どれくらい上下に動くか）
    public float amplitude = 0.1f;
    // 浮遊するスピード
    public float floatSpeed = 2f;
    // 回転するスピード
    public Vector3 rotateSpeed = new Vector3(15f, 30f, 45f);

    private Vector3 startLocalPosition;
    private float randomOffset;

    void Start()
    {
        // 吸収された時点での、スライム体内での初期位置（ローカル座標）を記憶
        startLocalPosition = transform.localPosition;

        // 全てのアイテムが同じタイミングで同じ動きをすると不自然なので、タイミングをランダムにずらす
        randomOffset = Random.Range(0f, 100f);

    }

    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime);

        float newY = startLocalPosition.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * amplitude;

        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);
    }
}