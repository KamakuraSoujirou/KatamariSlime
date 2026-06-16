using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlimeAI : MonoBehaviour
{
    [Header("移動設定")]
    public float jumpForce = 2.0f;     // 前に跳ぶ力
    public float upwardForce = 3.0f;   // 上に跳ぶ力
    public float jumpInterval = 1.5f;  // ジャンプする間隔（秒）
    public float rotationSpeed = 5.0f; // 振り向くスピード

    private Rigidbody rb;
    private Transform targetFood;
    private float jumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpTimer = jumpInterval;
    }

    void FixedUpdate()
    {
        // 常に一番近いエサを探す
        FindClosestFood();

        // エサが見つかった場合のみ行動する
        if (targetFood != null)
        {
            // 1. エサの方向を向く（Y軸の高さ違いを無視して、水平に振り向く）
            Vector3 direction = (targetFood.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                // なめらかに回転させる処理
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }

            // 2. 一定時間ごとにジャンプする
            jumpTimer -= Time.fixedDeltaTime;
            if (jumpTimer <= 0f)
            {
                JumpTowardsFood();
                jumpTimer = jumpInterval; // タイマーをリセット
            }
        }
    }

    void FindClosestFood()
    {
        // "Food"というタグがついているオブジェクトを全て取得
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        float closestDistance = Mathf.Infinity;
        Transform closestFood = null;

        // 最も距離が近いエサを計算する
        foreach (GameObject food in foods)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFood = food.transform;
            }
        }

        targetFood = closestFood;
    }

    void JumpTowardsFood()
    {
        // 前方向と上方向の力を合成して、瞬間的な力（Impulse）を加える
        Vector3 jumpVector = (transform.forward * jumpForce) + (Vector3.up * upwardForce);
        rb.AddForce(jumpVector, ForceMode.Force);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            collision.gameObject.GetComponent<Collider>().enabled = false;
            collision.gameObject.transform.SetParent(transform);
            collision.gameObject.transform.localPosition = Random.insideUnitSphere * 0.5f;
            collision.gameObject.transform.localScale *= 0.2f;

            collision.gameObject.AddComponent<FloatInSlime>();
            //transform.localScale += new Vector3(growthRate, growthRate, growthRate);

        }
    }
}