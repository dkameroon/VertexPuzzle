using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexGameManager : MonoBehaviour
{
    public static VertexGameManager Instance { get; private set; }
    public LevelData levelData;

    private float remainingTime;
    private bool isTimerRunning;

    private float previousMusicVolume;
    private float previousSoundVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckMusic();
        CheckSound();
        InitializeVolumeSliders();
        MainMenuUI.Instance.thankYouMessage.gameObject.SetActive(false);
        AdvancedSettingsUI.Instance.gameObject.SetActive(false);
        SettingsUI.Instance.gameObject.SetActive(false);
        MainMenuUI.Instance.okThankYouMessage.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.thankYouMessage.gameObject.SetActive(false);
            UIManager.Instance.OnOkButtonPressed();
        });
        MainMenuUI.Instance.playButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.StartGame();
            PauseUI.Instance.gameObject.SetActive(false);
            GameOverUI.Instance.gameObject.SetActive(false);
            LevelCompleteUI.Instance.gameObject.SetActive(false);
            LineDrawer.Instance.ResetLevel();
        });
        MainMenuUI.Instance.resetProgressButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.ResetProgress();
        });
        MainMenuUI.Instance.musicOffButton.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.musicOnButton.gameObject.SetActive(true);
            MainMenuUI.Instance.musicOffButton.gameObject.SetActive(false);
            ToggleMusic();
        });
        MainMenuUI.Instance.musicOnButton.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.musicOnButton.gameObject.SetActive(false);
            MainMenuUI.Instance.musicOffButton.gameObject.SetActive(true);
            ToggleMusic();
        });
        MainMenuUI.Instance.soundsOffButton.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.soundsOnButton.gameObject.SetActive(true);
            MainMenuUI.Instance.soundsOffButton.gameObject.SetActive(false);
            ToggleSound();
        });
        MainMenuUI.Instance.soundsOnButton.onClick.AddListener(() =>
        {
            MainMenuUI.Instance.soundsOnButton.gameObject.SetActive(false);
            MainMenuUI.Instance.soundsOffButton.gameObject.SetActive(true);
            ToggleSound();
        });
        SettingsUI.Instance.musicOffButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.musicOnButton.gameObject.SetActive(true);
            SettingsUI.Instance.musicOffButton.gameObject.SetActive(false);
            ToggleMusic();
        });
        SettingsUI.Instance.musicOnButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.musicOnButton.gameObject.SetActive(false);
            SettingsUI.Instance.musicOffButton.gameObject.SetActive(true);
            ToggleMusic();
        });
        SettingsUI.Instance.soundsOffButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.soundsOnButton.gameObject.SetActive(true);
            SettingsUI.Instance.soundsOffButton.gameObject.SetActive(false);
            ToggleSound();
        });
        SettingsUI.Instance.soundsOnButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.soundsOnButton.gameObject.SetActive(false);
            SettingsUI.Instance.soundsOffButton.gameObject.SetActive(true);
            ToggleSound();
        });
        SettingsUI.Instance.settingsCloseButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.gameObject.SetActive(false);
            AdvancedSettingsUI.Instance.gameObject.SetActive(false);
        });
        SettingsUI.Instance.advancedSettingsButton.onClick.AddListener(() =>
        {
            AdvancedSettingsUI.Instance.gameObject.SetActive(true);
            SettingsUI.Instance.gameObject.SetActive(false);
            
        });
        AdvancedSettingsUI.Instance.advancedSettingsCloseButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.gameObject.SetActive(true);
            AdvancedSettingsUI.Instance.gameObject.SetActive(false);
            
        });
        PauseUI.Instance.gameObject.SetActive(false);
        LevelCompleteUI.Instance.gameObject.SetActive(false);
        GameOverUI.Instance.gameObject.SetActive(false);
        GameUI.Instance.PauseGameButton.onClick.AddListener(() =>
        {
            //MusicManager.Instance.PauseMusic();
            Time.timeScale = 0f;
            PauseUI.Instance.gameObject.SetActive(true);
        });
        PauseUI.Instance.continueButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            Time.timeScale = 1f;
            PauseUI.Instance.gameObject.SetActive(false);
        });
        PauseUI.Instance.retryPauseButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            SoundEffectsManager.Instance.StopAllSounds();
            RetryLevel();
        });
        PauseUI.Instance.menuPauseButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.LoadMainMenu();
            MusicManager.Instance.ResumeMusic();
        });
        PauseUI.Instance.settingsButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.gameObject.SetActive(true);
            CheckMusic();
            CheckSound();
        });
        LevelCompleteUI.Instance.LevelCompleteSettingsButton.onClick.AddListener(() =>
        {
            SettingsUI.Instance.gameObject.SetActive(true);
        });
        LevelCompleteUI.Instance.LevelCompleteNextLevelButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            SoundEffectsManager.Instance.StopAllSounds();
            LevelCompleteUI.Instance.gameObject.SetActive(false);
            LevelManager.Instance.LoadNextLevel();
            LineDrawer.Instance.ResetMistakes();
            Time.timeScale = 1f;
        });
        LevelCompleteUI.Instance.LevelCompleteRetryButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            SoundEffectsManager.Instance.StopAllSounds();
            RetryLevel();
        });
        LevelCompleteUI.Instance.LevelCompleteMenuButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            LevelManager.Instance.LoadMainMenu();
        });
        GameOverUI.Instance.GameOverRetryButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            SoundEffectsManager.Instance.StopAllSounds();
            RetryLevel();
        });
        GameOverUI.Instance.GameOverMenuButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ResumeMusic();
            LevelManager.Instance.LoadMainMenu();
        });
    }

    private void Update()
    {
        if (isTimerRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            LevelObjects.Instance.TimerText.text = $"{minutes:D1}{seconds:D2}";

            if (remainingTime <= 0.01)
            {
                isTimerRunning = false;
                TimerEnded();
            }
        }
    }

    public void CheckMusic()
    {
        float currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SettingsUI.Instance.musicOffButton.gameObject.SetActive(currentMusicVolume == 0);
        SettingsUI.Instance.musicOnButton.gameObject.SetActive(currentMusicVolume > 0);
    }

    public void CheckSound()
    {
        float currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        SettingsUI.Instance.soundsOffButton.gameObject.SetActive(currentSoundVolume == 0);
        SettingsUI.Instance.soundsOnButton.gameObject.SetActive(currentSoundVolume > 0);
    }

    public void ToggleMusic()
{
    float currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

    if (currentMusicVolume > 0)
    {
        previousMusicVolume = currentMusicVolume;
        MusicManager.Instance.SetMusicVolume(0);
        PlayerPrefs.SetFloat("MusicVolume", 0);
        AdvancedSettingsUI.Instance.musicVolumeSlider.value = 0;
        MusicManager.Instance.PauseMusic();
    }
    else
    {
        MusicManager.Instance.SetMusicVolume(previousMusicVolume);
        PlayerPrefs.SetFloat("MusicVolume", previousMusicVolume);
        AdvancedSettingsUI.Instance.musicVolumeSlider.value = previousMusicVolume;
        MusicManager.Instance.ResumeMusic();
    }

    PlayerPrefs.Save();
    CheckMusic();
    UpdateMusicVolumeText(PlayerPrefs.GetFloat("MusicVolume"));
}

public void ToggleSound()
{
    float currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

    if (currentSoundVolume > 0)
    {
        previousSoundVolume = currentSoundVolume;
        SoundEffectsManager.Instance.SetSFXVolume(0);
        PlayerPrefs.SetFloat("SoundVolume", 0);
        AdvancedSettingsUI.Instance.soundVolumeSlider.value = 0;
    }
    else
    {
        SoundEffectsManager.Instance.SetSFXVolume(previousSoundVolume);
        PlayerPrefs.SetFloat("SoundVolume", previousSoundVolume);
        AdvancedSettingsUI.Instance.soundVolumeSlider.value = previousSoundVolume;
        SoundEffectsManager.Instance.PlaySound("correctConnectionSound");
    }

    PlayerPrefs.Save();
    CheckSound();
    UpdateSoundVolumeText(PlayerPrefs.GetFloat("SoundVolume"));
}

private void InitializeVolumeSliders()
{
    float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    previousMusicVolume = savedMusicVolume > 0 ? savedMusicVolume : 0.5f;
    AdvancedSettingsUI.Instance.musicVolumeSlider.value = savedMusicVolume;
    MusicManager.Instance.SetMusicVolume(savedMusicVolume);
    UpdateMusicVolumeText(savedMusicVolume);

    AdvancedSettingsUI.Instance.musicVolumeSlider.onValueChanged.RemoveAllListeners();
    AdvancedSettingsUI.Instance.musicVolumeSlider.onValueChanged.AddListener((value) =>
    {
        MusicManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        if (value > 0) previousMusicVolume = value;
        UpdateMusicVolumeText(value);
        CheckMusic();
    });

    float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
    previousSoundVolume = savedSoundVolume > 0 ? savedSoundVolume : 0.5f;
    AdvancedSettingsUI.Instance.soundVolumeSlider.value = savedSoundVolume;
    SoundEffectsManager.Instance.SetSFXVolume(savedSoundVolume);
    UpdateSoundVolumeText(savedSoundVolume);

    AdvancedSettingsUI.Instance.soundVolumeSlider.onValueChanged.RemoveAllListeners();
    AdvancedSettingsUI.Instance.soundVolumeSlider.onValueChanged.AddListener((value) =>
    {
        SoundEffectsManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
        if (value > 0) previousSoundVolume = value;
        UpdateSoundVolumeText(value);
        CheckSound();
    });
}

    public void RetryLevel()
    {
        LevelManager.Instance.RestartLevel();
        LineDrawer.Instance.ResetLevel();
        Time.timeScale = 1f;
        GameOverUI.Instance.gameObject.SetActive(false);
        LevelCompleteUI.Instance.gameObject.SetActive(false);
    }

    public void VertexGameOver()
    {
        foreach (var bulb in LevelObjects.Instance.LightBulbs)
        {
            bulb.GetComponent<Renderer>().material = LineDrawer.Instance.redBulbMaterial;
        }
        LineDrawer.Instance.MistakeCount = 3;
        Time.timeScale = 0f;
        GameOverUI.Instance.gameObject.SetActive(true);
        SoundEffectsManager.Instance.PlaySound("defeatSound");
    }

    private void TimerEnded()
    {
        VertexGameOver();
    }

    private void UpdateMusicVolumeText(float volume)
    {
        if (AdvancedSettingsUI.Instance.musicSliderText != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            AdvancedSettingsUI.Instance.musicSliderText.text = $"{percentage}%";
        }
    }

    private void UpdateSoundVolumeText(float volume)
    {
        if (AdvancedSettingsUI.Instance.soundsSliderText != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            AdvancedSettingsUI.Instance.soundsSliderText.text = $"{percentage}%";
        }
    }

    public void LevelComplete()
    {
        Time.timeScale = 0f;
        LevelCompleteUI.Instance.gameObject.SetActive(true);
        SoundEffectsManager.Instance.PlaySound("victorySound");
    }

    public void SetLevelTime(string levelName)
    {
        int timeLimit = levelData.GetTimeLimit(levelName);
        remainingTime = (timeLimit / 100 * 60) + (timeLimit % 100);
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void OnDestroy()
    {
        GameUI.Instance.PauseGameButton.onClick.RemoveAllListeners();
        PauseUI.Instance.continueButton.onClick.RemoveAllListeners();
        PauseUI.Instance.retryPauseButton.onClick.RemoveAllListeners();
        PauseUI.Instance.menuPauseButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.LevelCompleteNextLevelButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.LevelCompleteRetryButton.onClick.RemoveAllListeners();
        LevelCompleteUI.Instance.LevelCompleteMenuButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.GameOverRetryButton.onClick.RemoveAllListeners();
        GameOverUI.Instance.GameOverMenuButton.onClick.RemoveAllListeners();
    }
}
