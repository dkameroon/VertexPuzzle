using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    [Header("Sound Collection")]
    [SerializeField] private SoundEffectCollection soundEffectCollection;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string SFXVolumeKey = "SFXVolume";
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource();
            LoadSFXVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSource()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = soundEffectCollection.GetSound(soundName);
        if (clip == null)
        {
            Debug.LogWarning($"Sound '{soundName}' not found in the collection.");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
        SaveSFXVolume();
    }

    private void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadSFXVolume()
    {
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }
}