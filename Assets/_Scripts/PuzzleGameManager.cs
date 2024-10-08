using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
    public List<Vertex> vertices;
    private HashSet<(Vertex, Vertex)> visitedEdges = new HashSet<(Vertex, Vertex)>();

    public bool CanConnect(Vertex v1, Vertex v2)
    {
        return !visitedEdges.Contains((v1, v2)) && !visitedEdges.Contains((v2, v1));
    }

    public void MarkEdgeAsVisited(Vertex v1, Vertex v2)
    {
        visitedEdges.Add((v1, v2));
    }
}
