using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public static Vertex Instance { get; private set; }

    private bool isVisited = false;
    
    private void Awake()
    {
        Instance = this;
    }
    
    void OnMouseDown()
    {
        if (!isVisited)
        {
            isVisited = true;
            Debug.Log(isVisited);
        }
        else
        {
            isVisited = false;
            Debug.Log(isVisited);
        }
    }
    void Start()
    {

    }

    void Update()
    {
        
    }
}
