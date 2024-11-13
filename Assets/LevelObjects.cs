using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjects : MonoBehaviour
{
    public static LevelObjects Instance;
    
    public List<GameObject> LightBulbs;
    public Camera MainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LineDrawer.Instance.DrawPaths();
    }
}

