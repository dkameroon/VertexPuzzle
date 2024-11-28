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
    private bool isMusicPaused;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMusicSource();
            LoadMusicVolume();
            PlayCurrentTrack();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMusicSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
    }

    private void Update()
    {
        if (!isMusicPaused && !musicSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    public void PlayMusic(int trackIndex)
    {
        if (IsValidTrackIndex(trackIndex))
        {
            currentTrackIndex = trackIndex;
            PlayCurrentTrack();
        }
        else
        {
            Debug.LogWarning($"Invalid track index: {trackIndex}");
        }
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            isMusicPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isMusicPaused)
        {
            musicSource.UnPause();
            isMusicPaused = false;
        }
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
        {
            musicSource.volume = musicVolume;
        }
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

    private void PlayCurrentTrack()
    {
        if (musicTracks.Length > 0)
        {
            musicSource.clip = musicTracks[currentTrackIndex];
            musicSource.Play();
            isMusicPaused = false;
        }
        else
        {
            Debug.LogWarning("No tracks available in the music manager.");
        }
    }

    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayCurrentTrack();
    }

    private bool IsValidTrackIndex(int trackIndex)
    {
        return trackIndex >= 0 && trackIndex < musicTracks.Length;
    }
}
