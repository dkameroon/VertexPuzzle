using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer currentLineRenderer;
    private Vertex currentStartVertex;
    private Vertex lastVertex;
    private bool isDrawing = false;

    public Material lineMaterial;
    public Material pathMaterial;

    public float drawnLineWidth = 0.05f;
    public float pathLineWidth = 0.15f;
    public int pathSortingOrder = 0;
    public int drawnLineSortingOrder = 1;
    
    private HashSet<(Vertex, Vertex)> drawnLines = new HashSet<(Vertex, Vertex)>();
    private HashSet<(Vertex, Vertex)> drawnVisualPaths = new HashSet<(Vertex, Vertex)>();

    private Material originalMaterial;

    void Start()
    {
        foreach (var vertex in FindObjectsOfType<Vertex>())
        {
            DrawConnectedPaths(vertex);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDrawing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vertex vertex = hit.collider.GetComponent<Vertex>();
                
                if (vertex != null && (lastVertex == null || vertex == lastVertex))
                {
                    currentStartVertex = vertex;
                    CreateNewLineRenderer();
                    Vector3 startPosition = currentStartVertex.transform.position;
                    currentLineRenderer.SetPosition(0, startPosition);
                    currentLineRenderer.SetPosition(1, startPosition);
                    isDrawing = true;

                    originalMaterial = GetTopVertexMaterial(currentStartVertex);
                    SetTopVertexMaterial(currentStartVertex, lineMaterial);
                }
            }
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 currentPosition = GetMouseWorldPosition();
            currentPosition.y = currentStartVertex.transform.position.y;
            currentLineRenderer.SetPosition(1, currentPosition);
        }

        if (isDrawing && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vertex endVertex = hit.collider.GetComponent<Vertex>();
                if (endVertex != null && currentStartVertex.connectedVertices.Contains(endVertex))
                {
                    if (drawnLines.Contains((currentStartVertex, endVertex)) || drawnLines.Contains((endVertex, currentStartVertex)))
                    {
                        Destroy(currentLineRenderer.gameObject);
                    }
                    else
                    {
                        currentLineRenderer.SetPosition(1, endVertex.transform.position);
                        lastVertex = endVertex;
                        
                        drawnLines.Add((currentStartVertex, endVertex));
                        drawnLines.Add((endVertex, currentStartVertex));
                        
                        SetTopVertexMaterial(endVertex, lineMaterial);
                    }
                }
                else
                {
                    Destroy(currentLineRenderer.gameObject);
                    SetTopVertexMaterial(currentStartVertex, originalMaterial);
                }
            }
            else
            {
                Destroy(currentLineRenderer.gameObject);
                SetTopVertexMaterial(currentStartVertex, originalMaterial);
            }

            isDrawing = false;
        }
    }

    void CreateNewLineRenderer()
    {
        GameObject lineObject = new GameObject("Line");
        currentLineRenderer = lineObject.AddComponent<LineRenderer>();
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = drawnLineWidth;
        currentLineRenderer.endWidth = drawnLineWidth;
        currentLineRenderer.positionCount = 2;
        currentLineRenderer.useWorldSpace = true;

        currentLineRenderer.sortingOrder = drawnLineSortingOrder;
    }

    void DrawConnectedPaths(Vertex vertex)
    {
        foreach (Vertex connectedVertex in vertex.connectedVertices)
        {
            if (drawnVisualPaths.Contains((vertex, connectedVertex)) || drawnVisualPaths.Contains((connectedVertex, vertex)))
            {
                continue;
            }
            
            GameObject lineObject = new GameObject("VisualPathLine");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = pathMaterial;
            lineRenderer.startWidth = pathLineWidth;
            lineRenderer.endWidth = pathLineWidth;
            lineRenderer.positionCount = 2;

            lineRenderer.sortingOrder = pathSortingOrder;

            float offsetY = -0.1f;

            Vector3 startPosition = vertex.transform.position + new Vector3(0, offsetY, 0);
            Vector3 endPosition = connectedVertex.transform.position + new Vector3(0, offsetY, 0);
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            drawnVisualPaths.Add((vertex, connectedVertex));
            drawnVisualPaths.Add((connectedVertex, vertex));
        }
    }

    void SetTopVertexMaterial(Vertex vertex, Material material)
    {
        Transform topVertexTransform = vertex.transform.Find("TopVertex");
        if (topVertexTransform == null)
        {
            foreach (Transform child in vertex.transform)
            {
                if (child.name == "TopVertex")
                {
                    topVertexTransform = child;
                    break;
                }
            }
        }

        if (topVertexTransform != null)
        {
            Renderer renderer = topVertexTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
        }
        else
        {
            Debug.LogWarning("TopVertex not found under " + vertex.name);
        }
    }

    Material GetTopVertexMaterial(Vertex vertex)
    {
        Transform topVertexTransform = vertex.transform.Find("TopVertex");
        if (topVertexTransform != null)
        {
            Renderer renderer = topVertexTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                return renderer.material;
            }
        }
        return null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
