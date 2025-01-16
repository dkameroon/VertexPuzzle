using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance { get; private set; }
    
    [SerializeField] public Button advancedSettingsButton;
    [SerializeField] public Button settingsCloseButton;
    [SerializeField] public Button musicOffButton;
    [SerializeField] public Button musicOnButton;
    [SerializeField] public Button soundsOffButton;
    [SerializeField] public Button soundsOnButton;

    private void Awake()
    {
        Instance = this;
    }
}
