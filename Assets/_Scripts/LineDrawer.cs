using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer currentLineRenderer;  // Line being drawn
    private GameObject currentStartVertex;     // Starting vertex
    private bool isDrawing = false;            // Drawing state flag
    private float debounceTime = 0.5f;         // Debounce time to prevent multiple clicks
    private float lastClickTime = 0f;          // Last time the button was clicked
    private float clickStartTime = 0f;          // Start time of the current click

    public Material lineMaterial;              // Material for the line

    void Update()
    {
        // Check debounce time
        if (Time.time - lastClickTime < debounceTime)
            return;

        // Start drawing when the mouse is clicked
        if (Input.GetMouseButtonDown(0) && !isDrawing)
        {
            lastClickTime = Time.time;
            clickStartTime = Time.time;
            Debug.Log("Mouse Button Down - Start Drawing");

            // Raycast to find the vertex
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Vertex"))   
            {
                currentStartVertex = hit.collider.gameObject;

                // Ensure that no line already exists
                if (currentLineRenderer == null)
                {
                    // Create the new LineRenderer starting from the center of the vertex
                    CreateNewLineRenderer();
                    Vector3 vertexCenter = currentStartVertex.transform.position;
                    currentLineRenderer.SetPosition(0, vertexCenter);
                    currentLineRenderer.SetPosition(1, vertexCenter);  // Start both positions at the center

                    isDrawing = true;  // Set drawing state
                }
            }
        }

        // While drawing, update the second point of the line
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));   

            // Clamp the Y position to prevent the line from going below the test field
            currentPos = new Vector3(currentPos.x, Mathf.Max(1, currentPos.y), currentPos.z);
            currentLineRenderer.SetPosition(1, new Vector3(currentPos.x, 1, currentPos.z));  // Update the end position of the line, clamp Y to 1
        }

        // Finish drawing when the mouse is released and a minimum click duration has passed
        if (isDrawing && Input.GetMouseButtonUp(0) && Time.time - clickStartTime >= 0.2f)
        {
            Debug.Log("Mouse Button Up - Finish Drawing");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Vertex")    && hit.collider.gameObject != currentStartVertex)
                {
                    // Snap the line to the new vertex
                    currentLineRenderer.SetPosition(1, hit.collider.transform.position);
                }
                else
                {
                    // If not snapped to a valid vertex, destroy the line
                    Destroy(currentLineRenderer.gameObject);
                }
            }

            // Reset the drawing state and allow for a new line to be created
            isDrawing = false;
            currentLineRenderer = null;  // Ensure that only one LineRenderer exists at a time
        }
    }

    // Method to create a new LineRenderer
    void CreateNewLineRenderer()
    {
        Debug.Log("Creating New Line Renderer");

        GameObject newLine = new GameObject("Line");
        currentLineRenderer = newLine.AddComponent<LineRenderer>();

        // Set LineRenderer properties
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;

        // Initialize line positions
        currentLineRenderer.positionCount = 2;
        currentLineRenderer.useWorldSpace = true;
    }
}
