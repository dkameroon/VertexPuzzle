using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer currentLineRenderer; 
    private GameObject currentStartVertex;
    private bool isDrawing = false;
    private float debounceTime = 0.1f;
    private float lastClickTime = 0f; 

    public Material lineMaterial;
    
    private HashSet<(GameObject, GameObject)> traversedPaths = new HashSet<(GameObject, GameObject)>();
    
    private GameObject lastConnectedVertex = null;

    void Update()
    {
        if (Time.time - lastClickTime < debounceTime)
            return;
        
        if (Input.GetMouseButtonDown(0) && !isDrawing)
        {
            lastClickTime = Time.time;
            Debug.Log("Mouse Button Down - Start Drawing");
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Vertex"))
            {
                currentStartVertex = hit.collider.gameObject;
                
                if (lastConnectedVertex == null || currentStartVertex == lastConnectedVertex)
                {
                    if (currentLineRenderer == null)
                    {
                        CreateNewLineRenderer();
                        Vector3 vertexCenter = currentStartVertex.transform.position;
                        currentLineRenderer.SetPosition(0, vertexCenter);
                        currentLineRenderer.SetPosition(1, vertexCenter);

                        isDrawing = true;
                    }
                }
                else
                {
                    Debug.Log("You can only draw a line from the last connected vertex!");
                }
            }
        }
        
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            currentPos = new Vector3(currentPos.x, Mathf.Max(1, currentPos.y), currentPos.z);
            currentLineRenderer.SetPosition(1, new Vector3(currentPos.x, 1, currentPos.z));
        }
        
        if (isDrawing && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Button Up - Finish Drawing");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Vertex"))
            {
                GameObject endVertex = hit.collider.gameObject;
                
                if (endVertex != currentStartVertex && !IsPathAlreadyTraversed(currentStartVertex, endVertex))
                {
                    traversedPaths.Add((currentStartVertex, endVertex));
                    traversedPaths.Add((endVertex, currentStartVertex));
                    
                    currentLineRenderer.SetPosition(1, endVertex.transform.position);
                    
                    lastConnectedVertex = endVertex;
                }
                else
                {
                    Destroy(currentLineRenderer.gameObject);
                }
            }
            else
            {
                Destroy(currentLineRenderer.gameObject);
            }
            
            isDrawing = false;
            currentLineRenderer = null;
        }
    }

   
    void CreateNewLineRenderer()
    {
        Debug.Log("Creating New Line Renderer");

        GameObject newLine = new GameObject("Line");
        currentLineRenderer = newLine.AddComponent<LineRenderer>();
        
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;
        
        currentLineRenderer.positionCount = 2;
        currentLineRenderer.useWorldSpace = true;
    }
    
    bool IsPathAlreadyTraversed(GameObject start, GameObject end)
    {
        return traversedPaths.Contains((start, end));
    }
}
