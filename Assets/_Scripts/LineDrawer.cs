using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
  public LineRenderer lineRenderer;  // LineRenderer to draw the line
    private Transform startVertex;     // The vertex where the user starts the drag
    private bool isDragging = false;   // Whether the user is currently dragging

    void Start()
    {
        // Ensure the LineRenderer is set up properly
        lineRenderer.positionCount = 0;  // No points initially
    }

    void Update()
    {
        // Handle touch input for mobile or mouse input for testing in the editor
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Vector3 inputPosition;

            // Handle touch or mouse input
            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
            }
            else
            {
                inputPosition = Input.mousePosition;  // For testing in the editor
            }

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            // Detect if we hit a vertex
            if (Physics.Raycast(ray, out hit))
            {
                Vertex vertex = hit.collider.GetComponent<Vertex>();

                if (vertex != null)
                {
                    // When the user first touches a vertex, start the line from the vertex position
                    if (!isDragging)
                    {
                        StartLine(vertex.transform);
                    }
                }
            }

            // If dragging, update the end of the line to follow the finger/mouse
            if (isDragging)
            {
                UpdateLine(GetWorldPositionFromInput(inputPosition));
            }
        }

        // When the user releases the touch/mouse button
        if (Input.touchCount == 0 && !Input.GetMouseButton(0) && isDragging)
        {
            EndLine();
        }
    }

    // Starts the line from the vertex
    void StartLine(Transform vertex)
    {
        startVertex = vertex;
        isDragging = true;

        // Initialize line with two points
        lineRenderer.positionCount = 2;

        // Set the first point of the line to the vertex position
        lineRenderer.SetPosition(0, startVertex.position);  // First point is the vertex's position

        // Set the second point to the same vertex's position initially, but will update as we drag
        lineRenderer.SetPosition(1, startVertex.position);
    }

    // Updates the end point of the line while dragging
    void UpdateLine(Vector3 endPosition)
    {
        if (isDragging)
        {
            // Update only the second point of the line to follow the finger's position
            lineRenderer.SetPosition(1, endPosition);
        }
    }

    // Ends the line when the user releases the touch/mouse on a second vertex
    void EndLine()
    {
        // Finalize the line by checking for another vertex at the release position
        Vector3 inputPosition = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vertex endVertex = hit.collider.GetComponent<Vertex>();

            if (endVertex != null && endVertex.transform != startVertex)
            {
                // Complete the line between startVertex and endVertex
                lineRenderer.SetPosition(1, endVertex.transform.position);  // Snap the line to the second vertex
            }
            else
            {
                // If the user didn't end on a valid vertex, clear the line
                lineRenderer.positionCount = 0;  // Reset the line
            }
        }

        isDragging = false;  // Stop dragging
    }

    // Helper to convert screen position to world position
    Vector3 GetWorldPositionFromInput(Vector3 inputPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);  // Plane for 2D-like interaction on the XY axis
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);  // Convert the 2D input to 3D world space
        }

        return Vector3.zero;  // Fallback if raycast fails
    }
}
