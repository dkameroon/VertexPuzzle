using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer currentLineRenderer;
    private Vertex currentStartVertex;
    private Vertex lastVertex;  // Track the last vertex where a line was drawn
    private bool isDrawing = false;

    public Material lineMaterial;
    public Material pathMaterial;

    public float drawnLineWidth = 0.05f;  // Width for the drawn lines
    public float pathLineWidth = 0.02f;   // Width for the visual paths
    public int pathSortingOrder = 0;      // Order for visual paths (render below)
    public int drawnLineSortingOrder = 1; // Order for user-drawn lines (render above)

    // To store already drawn lines between vertex pairs (for user lines)
    private HashSet<(Vertex, Vertex)> drawnLines = new HashSet<(Vertex, Vertex)>();

    // To store already drawn visual paths
    private HashSet<(Vertex, Vertex)> drawnVisualPaths = new HashSet<(Vertex, Vertex)>();

    void Start()
    {
        // Initialize and draw visual paths between connected vertices at the start of the level
        foreach (var vertex in FindObjectsOfType<Vertex>())
        {
            DrawConnectedPaths(vertex);
        }
    }

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
            // Update the line's end position to follow the mouse, constrained to X and Z axes only
            Vector3 currentPosition = GetMouseWorldPosition();
            currentPosition.y = currentStartVertex.transform.position.y;  // Constrain the Y to the vertex's Y
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
        currentLineRenderer.startWidth = drawnLineWidth;  // Use drawn line width
        currentLineRenderer.endWidth = drawnLineWidth;
        currentLineRenderer.positionCount = 2;
        currentLineRenderer.useWorldSpace = true;

        // Set sorting order to ensure it's drawn above the visual paths
        currentLineRenderer.sortingOrder = drawnLineSortingOrder;
    }

    void DrawConnectedPaths(Vertex vertex)
    {
        foreach (Vertex connectedVertex in vertex.connectedVertices)
        {
            // Check if the path has already been drawn (in either direction)
            if (drawnVisualPaths.Contains((vertex, connectedVertex)) || drawnVisualPaths.Contains((connectedVertex, vertex)))
            {
                continue;  // Skip if path already exists
            }

            // Create the visual path line
            GameObject lineObject = new GameObject("VisualPathLine");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = pathMaterial;
            lineRenderer.startWidth = pathLineWidth;  // Use path line width
            lineRenderer.endWidth = pathLineWidth;
            lineRenderer.positionCount = 2;

            // Set sorting order so it renders below the user-drawn lines
            lineRenderer.sortingOrder = pathSortingOrder;

            // Set the positions of the line between this vertex and the connected vertex
            Vector3 startPosition = vertex.transform.position;
            Vector3 endPosition = connectedVertex.transform.position;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            // Add the path to the set of drawn visual paths (both directions)
            drawnVisualPaths.Add((vertex, connectedVertex));
            drawnVisualPaths.Add((connectedVertex, vertex));
        }
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
