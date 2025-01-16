using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [SerializeField] public static MainMenuUI Instance { get; private set; }
    [SerializeField] public Button playButton;
    [SerializeField] public Button resetProgressButton;
    
    [SerializeField] public GameObject thankYouMessage;
    [SerializeField] public Button okThankYouMessage;
    [SerializeField] public Button musicOffButton;
    [SerializeField] public Button musicOnButton;
    [SerializeField] public Button soundsOffButton;
    [SerializeField] public Button soundsOnButton;

    private void Awake()
    {
        Instance = this;
    }
}