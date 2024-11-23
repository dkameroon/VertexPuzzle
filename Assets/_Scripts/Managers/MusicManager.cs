using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Tracks")]
    [SerializeField] private AudioClip[] musicTracks;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;

    private const string MusicVolumeKey = "MusicVolume";
    private AudioSource musicSource;

    private int currentTrackIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource();
            LoadMusicVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        musicSource.clip = GetCurrentTrack();
        musicSource.Play();
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayNextTrackOrRepeat();
        }
    }

    public void PlayMusic(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicTracks.Length)
        {
            Debug.LogWarning("Track index out of range.");
            return;
        }

        currentTrackIndex = trackIndex;
        musicSource.clip = musicTracks[trackIndex];
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
        SaveMusicVolume();
    }

    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
    }

    private void LoadMusicVolume()
    {
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    private AudioClip GetCurrentTrack()
    {
        if (musicTracks.Length == 0) return null;
        return musicTracks[currentTrackIndex];
    }

    private void PlayNextTrackOrRepeat()
    {
        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No tracks available in the music manager.");
            return;
        }

        currentTrackIndex++;
        if (currentTrackIndex >= musicTracks.Length)
        {
            currentTrackIndex = 0;
        }

        musicSource.clip = GetCurrentTrack();
        musicSource.Play();
    }
}
