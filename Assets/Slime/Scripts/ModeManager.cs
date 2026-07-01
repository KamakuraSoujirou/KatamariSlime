using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    // モードの種類を定義
    public enum GameMode
    {
        SpawnSlime, // スライム出現モード
        FeedSlime   // エサやりモード
    }

    [Header("現在のモード")]
    public GameMode currentMode = GameMode.SpawnSlime;

    [Header("切り替えるUIの登録")]
    public GameObject spawnUI; // カラーピッカーなどを登録
    public GameObject feedUI;  // エサを選択するボタンなどを登録（あれば）

    // 「出現モード」のToggleが押された時に呼ばれる
    public void OnSpawnToggleChanged(bool isOn)
    {
        // isOn が true（選ばれた）の時だけ処理する
        if (isOn)
        {
            currentMode = GameMode.SpawnSlime;
            Debug.Log("スライム出現モードになりました");

            // 出現用のUIを表示し、エサ用のUIを隠す
            if (spawnUI != null) spawnUI.SetActive(true);
            if (feedUI != null) feedUI.SetActive(false);
        }
    }

    // 「エサやりモード」のToggleが押された時に呼ばれる
    public void OnFeedToggleChanged(bool isOn)
    {
        if (isOn)
        {
            currentMode = GameMode.FeedSlime;
            Debug.Log("エサやりモードになりました");

            // エサ用のUIを表示し、出現用のUIを隠す
            if (spawnUI != null) spawnUI.SetActive(false);
            if (feedUI != null) feedUI.SetActive(true);
        }
    }
}