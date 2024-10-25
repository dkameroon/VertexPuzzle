using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Bootstrap : MonoBehaviour
{
    private const int _scenesCount = 4;

    public void Awake()
    {
        Application.targetFrameRate = 60;

        
        for (int i = 1; i < _scenesCount; i++)
        {
            if (!SceneManager.GetSceneByBuildIndex(i).isLoaded)
            {
                SceneManager.LoadScene(i, LoadSceneMode.Additive);
            }
        }

        StartCoroutine(SetActiveSceneDelayed(_scenesCount - 1));
    }

    private IEnumerator SetActiveSceneDelayed(int loadScene)
    {
        yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(loadScene));
    }
    

#if UNITY_EDITOR
    [MenuItem("Bootstrap/Start Bootstrap")]
    private static void StartBootstrap()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        string scenePath = SceneUtility.GetScenePathByBuildIndex(0);
        if (!string.IsNullOrEmpty(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
#endif
}