using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlimeAI : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 1.5f;     // 滑るスピード
    public float rotationSpeed = 5.0f; // 振り向くスピード
    public float stopDistance = 0.3f;  // エサの手前で止まる距離

    private Rigidbody rb;
    [SerializeField] private Transform targetFood;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        FindClosestFood();

        if (targetFood != null)
        {
            // 1. エサの方向を計算（高さYのズレは無視して水平にする）
            Vector3 direction = (targetFood.position - transform.position);
            direction.y = 0;

            // スライムからエサまでの距離
            float distance = direction.magnitude;

            // 2. なめらかに振り向く処理
            if (distance > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }

            // 3. velocity（速度）を直接操作して移動する
            if (distance > stopDistance)
            {
                // 目標とする速度ベクトルを計算（向き × スピード）
                Vector3 targetVelocity = direction.normalized * moveSpeed;

                // 【重要】Y軸（上下）の速度は、現在の落下速度（重力）などをそのまま維持する
                // これをやらないと、空中に浮遊したり、床をすり抜けたりしてしまいます
                rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
            }
            else
            {
                // エサに到着したら、ピタッと止まるように水平方向の速度をゼロにする
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        else
        {
            // エサがない時も、滑り続けないように水平方向の速度をゼロにする
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // ぶつかった相手が「Food」タグなら、ターゲットかどうかに関わらず食べる
        if (collision.gameObject.CompareTag("Food"))
        {
            InSlime(collision.gameObject);
        }
    }


    void FindClosestFood()
    {
        // "Food"タグがついたエサを探して、一番近いものをターゲットにする
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;
        Transform closestFood = null;

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

    void InSlime(GameObject inSlime)
    {
        // 1. 即座にタグを変更し、他のスライムがターゲットにするのを防ぐ
        inSlime.tag = "InSlime";

        // もし食べているエサが現在のターゲットだった場合、ターゲットをリセットする
        if (targetFood != null && inSlime.transform == targetFood)
        {
            targetFood = null;
        }

        // 2. 物理演算と当たり判定を完全に停止する
        Collider col = inSlime.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rbFood = inSlime.GetComponent<Rigidbody>();
        if (rbFood != null) rbFood.isKinematic = true;

        // 3. スライムの子オブジェクトにして配置
        inSlime.transform.SetParent(transform);
        inSlime.transform.localPosition = Random.insideUnitSphere * 0.5f;
        inSlime.transform.localScale *= 0.2f;

        // 4. 浮遊スクリプトの追加
        inSlime.AddComponent<FloatInSlime>();
    }
}