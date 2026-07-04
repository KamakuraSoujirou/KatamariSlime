using UnityEngine;
using DG.Tweening;

public class UIPanelAnimator : MonoBehaviour
{
    [Header("アニメーション設定")]
    [SerializeField] float duration = 0.5f; // アニメーションの時間

    [SerializeField] Ease easeType = Ease.OutBack;// アニメーションのEase（動きのカーブ）

    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    bool isInitialized = false; // 初期化済みかどうかのフラグ

    void Awake()
    {
        Init();
    }

    // 初期化処理を独立させる
    private void Init()
    {
        if (isInitialized) return; // 既に初期化済みなら何もしない

        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // 開始時は見えない状態にしておく
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;

        isInitialized = true;
    }

    public void OpenPanel()
    {

        gameObject.SetActive(true);

        canvasGroup.DOKill();
        rectTransform.DOKill();

        canvasGroup.DOFade(1f, duration).SetEase(Ease.Linear);

        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, duration).SetEase(easeType);

    }

    public void ClosePanel()
    {
        canvasGroup.DOKill();
        rectTransform.DOKill();

        canvasGroup.DOFade(0f, duration * 0.8f).SetEase(Ease.Linear);

        rectTransform.DOScale(Vector3.one * 0.8f, duration * 0.8f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
