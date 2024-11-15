using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameUI gameUI;
    public PauseUI pauseUI;
    public SettingsUI settingsUI;
    public GameOverUI gameOverUI;
    public LevelCompleteUI levelCompleteUI;
    public MainMenuUI mainMenuUI;
    
    [SerializeField] private float messageDuration = 100f;
    
    private bool okPressed = false;

    private void Awake()
    {
        Instance = this;
    }
    

    public void ShowThankYouMessage()
    {
        MainMenuUI.Instance.thankYouMessage.gameObject.SetActive(true);
        MainMenuUI.Instance.playButton.gameObject.SetActive(false);
        MainMenuUI.Instance.resetProgressButton.gameObject.SetActive(false);
        okPressed = false;
        StartCoroutine(WaitForOkOrTimeout());
    }

    private IEnumerator WaitForOkOrTimeout()
    {
        float elapsedTime = 0f;

        while (elapsedTime < messageDuration && !okPressed)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        HideThankYouMessage();
    }

    public void OnOkButtonPressed()
    {
        okPressed = true;
    }

    public void HideThankYouMessage()
    {
        MainMenuUI.Instance.thankYouMessage.gameObject.SetActive(false);
        MainMenuUI.Instance.playButton.gameObject.SetActive(true);
        MainMenuUI.Instance.resetProgressButton.gameObject.SetActive(true);
    }
}