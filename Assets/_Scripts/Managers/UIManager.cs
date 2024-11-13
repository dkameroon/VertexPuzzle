using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameUI gameUI;
    public PauseUI pauseUI;
    public SettingsUI settingsUI;
    public GameOverUI gameOverUI;
    public LevelCompleteUI levelCompleteUI;
    public MainMenuUI mainMenuUI;

    private void Awake()
    {
        Instance = this;
    }
}