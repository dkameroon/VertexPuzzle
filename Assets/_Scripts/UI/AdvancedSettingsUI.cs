using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedSettingsUI : MonoBehaviour
{
    public static AdvancedSettingsUI Instance { get; private set; }
    
    [SerializeField] public Button advancedSettingsCloseButton;

    [SerializeField] public Slider musicVolumeSlider;
    [SerializeField] public Slider soundVolumeSlider;

    [SerializeField] public TextMeshProUGUI musicSliderText;
    [SerializeField] public TextMeshProUGUI soundsSliderText;

    private void Awake()
    {
        Instance = this;
    }
}
