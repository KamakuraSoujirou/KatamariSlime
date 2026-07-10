using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

public class RandomGrassSpawner : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject effectPrefab; // 草のプレハブ
    [SerializeField] private int spawnCountPerPlane = 5; // 1つの平面に生やす草の数
    [SerializeField] private Vector3 spawnSize = Vector3.one;

    private void OnEnable()
    {
        if (planeManager != null) planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        if (planeManager != null) planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            // 床が追加されたら、少し待ってから草を生やす処理を開始
            StartCoroutine(SpawnRandomGrassRoutine(plane));
        }
    }

    private IEnumerator SpawnRandomGrassRoutine(ARPlane plane)
    {
        // ARの床メッシュが作られるまで少し待つ
        yield return new WaitForSeconds(0.5f);

        if (plane == null)
        {
            yield break; // もし消えていたら、ここで草を生やす処理を中止する
        }
        // 床の当たり判定（MeshCollider）を取得
        MeshCollider planeCollider = plane.GetComponent<MeshCollider>();
        if (planeCollider == null)
        {
            Debug.LogWarning("AR PlaneにMeshColliderがありません！プレハブを確認してください。");
            yield break;
        }

        int spawnedCount = 0;
        int maxAttempts = 30; // 無限ループを防ぐための保険（最大30回まで探す）

        for (int i = 0; i < maxAttempts; i++)
        {
            // 目標の数に達したら終了
            if (spawnedCount >= spawnCountPerPlane) break;

            // 1. 床の大まかなサイズ（縦横）からランダムな位置を計算
            Vector2 planeSize = plane.size;
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.y / 2, planeSize.y / 2);

            // ローカル座標（床の中心を0とした座標）を、ワールド座標（実際のAR空間の座標）に変換
            Vector3 randomLocalPos = new Vector3(randomX, 0, randomZ);
            Vector3 randomWorldPos = plane.transform.TransformPoint(randomLocalPos);

            // 2. その位置の「50cm上」から「下」に向かってRay（光線）を作る
            Vector3 rayStartPos = randomWorldPos + Vector3.up * 0.5f;
            Ray ray = new Ray(rayStartPos, Vector3.down);
            RaycastHit hit;

            // 3. この床のコライダーに対してRayを飛ばし、当たったかチェック
            if (planeCollider.Raycast(ray, out hit, 1.0f))
            {
                // 当たった！（＝床の範囲内）ので、草を生成する
                SpawnEffect(hit.point, plane.transform);
                spawnedCount++;
            }
        }
    }

    private void SpawnEffect(Vector3 spawnPosition, Transform parentPlane)
    {
        GameObject effect = Instantiate(effectPrefab, spawnPosition, Quaternion.identity);
        effect.transform.SetParent(parentPlane);

        // 🌟 おまけ：草の向き（Y軸）をランダムに回転させると自然に見えます
        float randomRotationY = Random.Range(0f, 360f);
        effect.transform.Rotate(0, randomRotationY, 0);

        // じわっと大きくする
        StartCoroutine(ScaleUpRoutine(effect.transform));
    }


    private IEnumerator ScaleUpRoutine(Transform targetTransform)
    {
        float time = 0;
        float scaleDuration = 0.5f;
        targetTransform.localScale = Vector3.zero;
        Vector3 targetScale = spawnSize;

        while (time < scaleDuration)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            time += Time.deltaTime;
            float t = time / scaleDuration;
            targetTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        targetTransform.localScale = targetScale;
    }
}