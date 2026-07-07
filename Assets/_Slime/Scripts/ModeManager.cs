using UnityEngine;
// UIPanelAnimator がUIの機能を使っているなら using UnityEngine.UI; などが必要な場合があります

public class ModeManager : MonoBehaviour
{
    // モードの種類を定義
    public enum GameMode
    {
        SpawnSlime, // スライム出現モード
        FeedSlime,  // エサやりモード
        Photoshoot  // 写真撮影モード
    }

    [Header("現在のモード")]
    public GameMode currentMode = GameMode.SpawnSlime;

    [Header("切り替えるUIの登録")]
    public GameObject spawnUI;
    public GameObject feedUI;
    public GameObject photoshootUI;

    // ＝＝＝ Toggleから呼ばれるメソッド群（めちゃくちゃ短くなります） ＝＝＝
    public void OnSpawnToggleChanged(bool isOn)
    {
        if (isOn) ChangeMode(GameMode.SpawnSlime);
    }

    public void OnFeedToggleChanged(bool isOn)
    {
        if (isOn) ChangeMode(GameMode.FeedSlime);
    }

    public void OnPhotoshootToggleChanged(bool isOn)
    {
        if (isOn) ChangeMode(GameMode.Photoshoot);
    }

    // ＝＝＝ ここから下が整理された共通処理 ＝＝＝

    // モードを切り替え、すべてのUIの開閉を一括で管理するメソッド
    private void ChangeMode(GameMode newMode)
    {
        currentMode = newMode;
        Debug.Log($"{currentMode} モードになりました");

        // 対象のモードなら true(開く)、それ以外なら false(閉じる) を渡す
        ToggleUIPanel(spawnUI, newMode == GameMode.SpawnSlime);
        ToggleUIPanel(feedUI, newMode == GameMode.FeedSlime);
        ToggleUIPanel(photoshootUI, newMode == GameMode.Photoshoot);
    }

    // 1つのUIを開閉するための便利メソッド（ヘルパーメソッド）
    private void ToggleUIPanel(GameObject ui, bool shouldOpen)
    {
        // もしUIが未設定なら、ここで処理を抜ける（このメソッドから抜けるだけなので安全！）
        if (ui == null) return;

        UIPanelAnimator animator = ui.GetComponent<UIPanelAnimator>();

        // アニメーターが付いている場合のみ実行
        if (animator != null)
        {
            if (shouldOpen)
            {
                animator.OpenPanel();
            }
            else
            {
                animator.ClosePanel();
            }
        }
    }
}