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
    public GameObject mainMenuUI;
    public GameObject gameUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameUI.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(4);
        mainMenuUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
    }
}