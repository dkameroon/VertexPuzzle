using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private const string ProgressKey = "LastCompletedLevel";
    private string activeLevelName;
    private bool isLoaded = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        string lastCompletedLevel = PlayerPrefs.GetString(ProgressKey, "Level 1");
        LoadLevel(lastCompletedLevel);
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(activeLevelName))
        {
            int currentLevelIndex = SceneManager.GetSceneByName(activeLevelName).buildIndex;
            int nextLevelIndex = currentLevelIndex + 1;

            if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                string nextLevelName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(nextLevelIndex));
                CompleteLevel(activeLevelName);
                LoadLevel(nextLevelName);
            }
            else
            {
                Debug.Log("No more levels available. Returning to Main Menu.");
                LoadMainMenu();
            }
        }
    }

    public void RestartLevel()
    {
        if (!string.IsNullOrEmpty(activeLevelName))
        {
            LoadLevel(activeLevelName);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        UIManager.Instance.mainMenuUI.gameObject.SetActive(true);
        UIManager.Instance.gameUI.gameObject.SetActive(false);
    }

    private void LoadLevel(string levelName)
    {
        SceneUnload();
    
        if (SceneManager.GetSceneByName("MainMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    
        activeLevelName = levelName;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        asyncLoad.completed += (asyncOp) =>
        {
            Scene loadedScene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(loadedScene);
            
            Light[] lights = loadedScene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<Light>())
                .ToArray();
            foreach (var light in lights)
            {
                light.enabled = true;
            }

            UIManager.Instance.mainMenuUI.gameObject.SetActive(false);
            UIManager.Instance.gameUI.gameObject.SetActive(true);
            UIManager.Instance.pauseUI.gameObject.SetActive(false);

            string levelNumberStr = levelName.Replace("Level ", "");
            int levelNumber = int.Parse(levelNumberStr);
            string formattedLevel = string.Format("LEVEL {0:D2}", levelNumber);
            GameUI.Instance.levelText.text = formattedLevel;

            isLoaded = true;
        };
    }

    private void SceneUnload()
    {
        Scene[] activeScenes = new Scene[SceneManager.sceneCount];
        for (int i = 0; i < activeScenes.Length; i++)
        {
            activeScenes[i] = SceneManager.GetSceneAt(i);
        }
        foreach (Scene scene in activeScenes)
        {
            if (scene.name.StartsWith("Level "))
            {
                SceneManager.UnloadSceneAsync(scene.name);
            }
        }
    }

    public void CompleteLevel(string currentLevel)
    {
        PlayerPrefs.SetString(ProgressKey, currentLevel);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetString(ProgressKey, "Level 1");
        PlayerPrefs.Save();
    }
}
