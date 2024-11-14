using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance;
    
    public Button LevelCompleteNextLevelButton;
    public Button LevelCompleteSettingsButton;
    public Button LevelCompleteRetryButton;
    public Button LevelCompleteMenuButton;

    private void Awake()
    {
        Instance = this;
    }
}
