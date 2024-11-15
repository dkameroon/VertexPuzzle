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
        MainMenuUI.Instance.thankYouMessage.gameObject.SetActive(false);
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
        /*SettingsUI.Instance.gameObject.SetActive(false);*/
        PauseUI.Instance.gameObject.SetActive(false);
        LevelCompleteUI.Instance.gameObject.SetActive(false);
        GameOverUI.Instance.gameObject.SetActive(false);
        GameUI.Instance.PauseGameButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            PauseUI.Instance.gameObject.SetActive(true);
        });
        PauseUI.Instance.continueButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            PauseUI.Instance.gameObject.SetActive(false);
        });
        PauseUI.Instance.retryPauseButton.onClick.AddListener(() =>
        {
            RetryLevel();
        });
        PauseUI.Instance.menuPauseButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.LoadMainMenu();
        });
        LevelCompleteUI.Instance.LevelCompleteNextLevelButton.onClick.AddListener(() =>
        {
            LevelCompleteUI.Instance.gameObject.SetActive(false);
            LevelManager.Instance.LoadNextLevel();
            LineDrawer.Instance.ResetMistakes();
            Time.timeScale = 1f;
        });
        LevelCompleteUI.Instance.LevelCompleteRetryButton.onClick.AddListener(() =>
        {
            RetryLevel();
        });
        LevelCompleteUI.Instance.LevelCompleteMenuButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.LoadMainMenu();
        });
        GameOverUI.Instance.GameOverRetryButton.onClick.AddListener(() =>
        {
            RetryLevel();
        });
        GameOverUI.Instance.GameOverMenuButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.LoadMainMenu();
        });
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

    private void TimerEnded()
    {
        VertexGameOver();
    }

    public void LevelComplete()
    {
        Time.timeScale = 0f;
        LevelCompleteUI.Instance.gameObject.SetActive(true);
        Debug.Log("Level Complete!");
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
