using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : BaseUI
{
    public static PauseUI Instance { get; private set; }
    
    [SerializeField] public Button continueButton;
    [SerializeField] public Button retryPauseButton;
    [SerializeField] public Button settingsButton;
    [SerializeField] public Button menuPauseButton;

    private void Awake()
    {
        Instance = this;
    }

}