using UnityEngine;

public class Audio2DManager : MonoBehaviour
{
    // どこからでもアクセスできる「窓口」を作る
    public static Audio2DManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        // 自分が唯一の存在かチェック
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 既に存在していたら自分を消す
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}