using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    public static GameUI Instance { get; private set; }
    
    [SerializeField] public Button pauseGameButton;
    [SerializeField] public TextMeshProUGUI levelText;


    private void Awake()
    {
        Instance = this;
    }
    
}