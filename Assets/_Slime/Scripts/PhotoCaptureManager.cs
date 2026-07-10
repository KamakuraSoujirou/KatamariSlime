using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCaptureManager : MonoBehaviour
{
    [Header("撮影時に一瞬だけ隠すUI（シャッターボタン等）")]
    public GameObject[] uisToHide;

    [Header("FadeManager")]
    [SerializeField] private FadeManager fadeManager;

    [Header("一覧表示用の設定")]
    [SerializeField] Transform contentContainer;
    [SerializeField] GameObject photoItemPrefab;

    private List<string> capturedPhotoPaths = new List<string>();

    public void TakePhoto()
    {
        StartCoroutine(CaptureRoutine());
    }

    private IEnumerator CaptureRoutine()
    {
        foreach (GameObject ui in uisToHide)
        {
            if (ui != null) ui.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        Texture2D ss = ScreenCapture.CaptureScreenshotAsTexture();

        foreach (GameObject ui in uisToHide)
        {
            if (ui != null) ui.SetActive(true);
        }

        string albumName = "SlimeAR"; // スマホ内に作られるアルバム（フォルダ）名
        string fileName = "Slime_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        string localPath = Path.Combine(Application.persistentDataPath, fileName);//アプリ内表示用のローカル保存
        Debug.Log("<color=cyan>【テスト】保存先パス: " + localPath + "</color>");

        byte[] pngData = ss.EncodeToPNG();
        File.WriteAllBytes(localPath, pngData);

        capturedPhotoPaths.Add(localPath);


#if !UNITY_EDITOR
        // NativeGalleryを使って保存処理を実行
        NativeGallery.SaveImageToGallery(ss, albumName, fileName, (success, path) =>
        {
            if (success)
            {
                Debug.Log("写真アプリに保存しました！ パス: " + path);
            }
            else
            {
                Debug.Log("写真の保存に失敗しました...");
            }

            Destroy(ss);
        });
#else
        // Unityエディターでのみ実行されるブロック
        Debug.Log("※エディター環境のため、カメラロールへの保存処理をスキップします。");
        Destroy(ss);
#endif

        if (fadeManager != null)
            fadeManager.PhotoFlash();
    }

    public void ViewPhotosInApp()
    {
        // 1. スクロールビュー内の古い表示をすべてクリア
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 記憶しているパスの数だけ、一覧に写真アイテムを生成
        foreach (string path in capturedPhotoPaths)
        {
            // ファイルが実際に存在するか確認（念のため）
            if (File.Exists(path))
            {
                CreatePhotoItem(path);
            }
        }
    }

    // 写真アイテムを動的に生成してRawImageにテクスチャを貼り付ける
    private void CreatePhotoItem(string path)
    {
        // プレハブをContentの子として生成
        GameObject newItem = Instantiate(photoItemPrefab, contentContainer);

        // 子オブジェクトからRawImageコンポーネントを探す
        RawImage rawImage = newItem.GetComponentInChildren<RawImage>();

        if (rawImage != null)
        {
            // 画像ファイルをバイトデータとして読み込む
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); // バイトデータから画像を復元

            // RawImageにセットして表示
            rawImage.texture = tex;
        }
    }
}