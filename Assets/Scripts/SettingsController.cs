using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private const string MasterVolume = "MasterVolume";
    private const string SFXVolume = "SFXVolume";
    private const string Fullscreen = "Fullscreen";
    private const string ScreenShake = "ScreenShake";

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeS;
    [SerializeField] private Slider SFXVolumeS;

    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle screenShakeToggle;

    public static bool isScreenShake { get; private set; } = true;

    private void Awake()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolume, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolume, 1f);

        bool fullscreen = PlayerPrefs.GetInt(Fullscreen, Screen.fullScreen ? 1 : 0) == 1;
        bool screenshake = PlayerPrefs.GetInt(ScreenShake, 1) == 1;

        AudioListener.volume = masterVolume;
        SoundEffectManager.SetVolume(sfxVolume);

        Screen.fullScreen = fullscreen;
        isScreenShake = screenshake;

        if (masterVolumeS != null)
            masterVolumeS.SetValueWithoutNotify(masterVolume);

        if (SFXVolumeS != null)
            SFXVolumeS.SetValueWithoutNotify(sfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        if (screenShakeToggle != null)
            screenShakeToggle.SetIsOnWithoutNotify(screenshake);
    }

    private void OnEnable()
    {
        if(masterVolumeS != null) 
        {
            masterVolumeS.onValueChanged.AddListener(SetMasterVolume);
        }

        if (SFXVolumeS != null)
        {
            SFXVolumeS.onValueChanged.AddListener(SetSFXVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.AddListener(SetScreenShake);
        }
    }

    private void OnDisable()
    {
        if (masterVolumeS != null)
        {
            masterVolumeS.onValueChanged.RemoveListener(SetMasterVolume);
        }

        if (SFXVolumeS != null)
        {
            SFXVolumeS.onValueChanged.RemoveListener(SetSFXVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }

        if (screenShakeToggle != null)
        {
            screenShakeToggle.onValueChanged.RemoveListener(SetScreenShake);
        }
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        AudioListener.volume = volume;

        PlayerPrefs.SetFloat(MasterVolume, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        SoundEffectManager.SetVolume(volume);

        PlayerPrefs.SetFloat(SFXVolume, volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt(Fullscreen, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetScreenShake(bool enabled)
    {
        isScreenShake = enabled;

        PlayerPrefs.SetInt(ScreenShake, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
