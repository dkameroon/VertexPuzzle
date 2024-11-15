using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    public static MainMenuUI Instance { get; private set; }
    public Button playButton;
    public Button resetProgressButton;
    
    public GameObject thankYouMessage;
    public Button okThankYouMessage;

    private void Awake()
    {
        Instance = this;
    }
}