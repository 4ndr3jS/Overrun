using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private static SoundEffectLibrary soundEffectLibrary;
    private static AudioSource audioSource;
    private static AudioSource randomPitchAudioSource;
    private static AudioSource voiceAudioSource;

    private static float currentVolume = 1f;

    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSource = audioSources[0];
            randomPitchAudioSource = audioSources[1];
            voiceAudioSource = audioSources[2];
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            ApplyVolume();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        if(sfxSlider != null){
            sfxSlider.onValueChanged.AddListener(delegate { onValueChanged(); });
        }
    }

    public static void Play(string soundName, bool randomPitch = false, float minPitch = 1.5f, float maxPitch = 2f)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if(audioClip != null)
        {
            if (randomPitch)
            {
                randomPitchAudioSource.pitch = Random.Range(1.5f, 2f);
                randomPitchAudioSource.PlayOneShot(audioClip);
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
    }

    public static void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);

        ApplyVolume();
    }

    private static void ApplyVolume()
    {
        if (audioSource == null || randomPitchAudioSource == null || voiceAudioSource == null)
            return;

        audioSource.volume = currentVolume;
        randomPitchAudioSource.volume = currentVolume;
        voiceAudioSource.volume = currentVolume;
    }

    public void onValueChanged()
    {
        SetVolume(sfxSlider.value);
    }

    public static void PlayVoice(AudioClip audioClip, float pitch = 1f)
    {
        voiceAudioSource.pitch = pitch;
        voiceAudioSource.PlayOneShot(audioClip);
    }
}
