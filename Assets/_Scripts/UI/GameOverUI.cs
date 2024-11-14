using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    
    public Button GameOverRetryButton;
    public Button GameOverMenuButton;

    private void Awake()
    {
        Instance = this;
    }
}
