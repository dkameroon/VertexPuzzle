using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelObjects : MonoBehaviour
{
    public static LevelObjects Instance;
    
    public List<GameObject> LightBulbs;
    public Camera MainCamera;
    public TextMeshProUGUI TimerText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LineDrawer.Instance.DrawPaths();
    }
}

