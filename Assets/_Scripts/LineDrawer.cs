using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer currentLineRenderer;
    private Vertex currentStartVertex;
    private Vertex lastVertex;  // Track the last vertex where a line was drawn
    private bool isDrawing = false;

    public Material lineMaterial;

    // To store already drawn lines between vertex pairs
    private HashSet<(Vertex, Vertex)> drawnLines = new HashSet<(Vertex, Vertex)>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDrawing)
        {
            // Detect the starting vertex
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vertex vertex = hit.collider.GetComponent<Vertex>();
                
                // Ensure that drawing can only start from the last vertex, or if no line has been drawn, from any vertex
                if (vertex != null && (lastVertex == null || vertex == lastVertex))
                {
                    currentStartVertex = vertex;
                    CreateNewLineRenderer();
                    Vector3 startPosition = currentStartVertex.transform.position;
                    currentLineRenderer.SetPosition(0, startPosition);
                    currentLineRenderer.SetPosition(1, startPosition);
                    isDrawing = true;
                }
            }
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            // Update the line's end position to follow the mouse
            Vector3 currentPosition = GetMouseWorldPosition();
            currentLineRenderer.SetPosition(1, currentPosition);
        }

        if (isDrawing && Input.GetMouseButtonUp(0))
        {
            // Finish drawing, check for valid connection
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vertex endVertex = hit.collider.GetComponent<Vertex>();
                if (endVertex != null && currentStartVertex.connectedVertices.Contains(endVertex))
                {
                    // Check if the line between these two vertices already exists
                    if (drawnLines.Contains((currentStartVertex, endVertex)) || drawnLines.Contains((endVertex, currentStartVertex)))
                    {
                        // Line already exists, cancel the drawing
                        Destroy(currentLineRenderer.gameObject);
                    }
                    else
                    {
                        // Valid new connection, finalize the line
                        currentLineRenderer.SetPosition(1, endVertex.transform.position);
                        lastVertex = endVertex;  // Update the last vertex

                        // Add the connection to the set of drawn lines (both directions)
                        drawnLines.Add((currentStartVertex, endVertex));
                        drawnLines.Add((endVertex, currentStartVertex));
                    }
                }
                else
                {
                    // Invalid connection, remove the line
                    Destroy(currentLineRenderer.gameObject);
                }
            }
            else
            {
                // No vertex hit, remove the line
                Destroy(currentLineRenderer.gameObject);
            }

            isDrawing = false;
        }
    }

    void CreateNewLineRenderer()
    {
        GameObject lineObject = new GameObject("Line");
        currentLineRenderer = lineObject.AddComponent<LineRenderer>();
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = 0.05f;
        currentLineRenderer.endWidth = 0.05f;
        currentLineRenderer.positionCount = 2;
        currentLineRenderer.useWorldSpace = true;
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
