using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    private readonly string BGM_KEY = "BGM";
    private readonly string SE_KEY = "SE";

    private void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 0.5f);
        float seVolume = PlayerPrefs.GetFloat(SE_KEY, 0.5f);


        if (BGMSlider != null) BGMSlider.value = bgmVolume;
        if (SESlider != null) SESlider.value = seVolume;
    }

    public void SetBGM(float volume)
    {
        ApplyVolume("BGM", volume);
        SaveVolume(BGM_KEY, volume);
    }

    public void SetSE(float volume)
    {
        ApplyVolume("SE", volume);
        SaveVolume(SE_KEY, volume);
    }

   void ApplyVolume(string paramName,float volume)
   {
        // Sliderの値(0~1)をデシベル(-80~0)に変換する
        // volumeが0の時に計算エラー(マイナス無限大)になるのを防ぐため、0.0001fより大きいかチェック
        float db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        audioMixer.SetFloat(paramName, db);
        Debug.Log(paramName + " Volume set to: " + db + " dB");
    }

   void SaveVolume(string key, float volume)
    {
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.Save();
    }

}