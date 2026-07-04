using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    [Header("フェード用の黒い画像")]
    public Image fadeImage;

    [Header("フェードにかかる時間（秒）")]
    public float fadeDuration = 1.0f;

    void Start()
    {
        // --- フェードイン（真っ黒から透明へ） ---

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 1); // 確実にはじめを真っ黒にしておく

        // DOFade(目標の透明度, かかる時間)
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            // .OnComplete の中身は、アニメーションが「終わった瞬間」に実行されます
            fadeImage.gameObject.SetActive(false);
        });

    }

    // スタートボタンなどから呼び出されるメソッド
    public void FadeOutAndLoadScene(string sceneName)
    {
        // --- フェードアウト（透明から真っ黒へ） ---

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0); // 確実にはじめを透明にしておく

        // 1f（真っ黒）に向けてフェードし、終わったらシーンを切り替える
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}