using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelTimeData
    {
        public string levelName;
        public int timeLimit;
    }

    public List<LevelTimeData> levelTimeList;
    
    public int GetTimeLimit(string levelName)
    {
        foreach (LevelTimeData data in levelTimeList)
        {
            if (data.levelName == levelName)
            {
                return data.timeLimit;
            }
        }
        Debug.LogWarning($"Time limit for level '{levelName}' not found. Using default time limit.");
        return 60;
    }
}