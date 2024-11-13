using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    public static MainMenuUI Instance { get; private set; }
    public Button playButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void StartGame()
    {
        LevelManager.Instance.StartGame();
    }
}