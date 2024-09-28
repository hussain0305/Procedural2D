using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS_MazeGeneration : MonoBehaviour
{
    public int gridSize = 20;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject startPrefab;     // Prefab for the start node
    public GameObject destinationPrefab; // Prefab for the destination node
    public int minPathLength = 10;     // Minimum path length between start and end

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0), // Left
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(0, 1)   // Up
    };

    private int[,] grid; // 1 for wall, 0 for path
    private Stack<Vector2Int> stack = new Stack<Vector2Int>(); // Stack for DFS
    private Vector2Int startNode;
    private Vector2Int destinationNode;
    private int currentPathLength = 0;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        grid = new int[gridSize, gridSize];

        // Initialize the grid with walls
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = 1; // 1 means a wall
            }
        }

        // Set start and end nodes
        startNode = new Vector2Int(0, 0);  // Top-left corner
        destinationNode = new Vector2Int(gridSize - 1, gridSize - 1); // Bottom-right corner

        // Step 1: Start DFS from the startNode
        stack.Push(startNode);
        grid[startNode.x, startNode.y] = 0; // Mark start node as part of the path

        DFS();

        // Ensure minimum path length is met
        StretchPathToEnd();
    }

    // Depth-First Search (DFS) to generate the maze
    void DFS()
    {
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                // Randomly pick a neighbor to visit
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];

                // Mark the neighbor as part of the path
                grid[next.x, next.y] = 0;
                stack.Push(next);
                currentPathLength++;
            }
            else
            {
                // No unvisited neighbors, backtrack
                stack.Pop();
            }
        }
    }

    // Get unvisited neighbors (i.e., walls) around the current cell
    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            Vector2Int next = current + dir;

            // Ensure the neighbor is within bounds and still a wall
            if (IsInBounds(next.x, next.y) && grid[next.x, next.y] == 1)
            {
                // Ensure the neighbor has at least two walls (to prevent cycles)
                if (CountAdjacentWalls(next) >= 3)
                {
                    neighbors.Add(next);
                }
            }
        }

        return neighbors;
    }

    // Count how many walls are adjacent to a cell (helps prevent cycles)
    int CountAdjacentWalls(Vector2Int cell)
    {
        int wallCount = 0;
        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (IsInBounds(neighbor.x, neighbor.y) && grid[neighbor.x, neighbor.y] == 1)
            {
                wallCount++;
            }
        }
        return wallCount;
    }

    // Ensure the path between start and end has the minimum required length
    void StretchPathToEnd()
    {
        Vector2Int current = destinationNode;

        while (currentPathLength < minPathLength)
        {
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                // Stretch the path by visiting another neighbor
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                grid[next.x, next.y] = 0;
                currentPathLength++;
                current = next;
            }
            else
            {
                // If no unvisited neighbors are available, stop stretching
                break;
            }
        }
    }

    // Check if a cell is within grid bounds
    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    bool IsStartingOrDestinationNode(Vector2Int position)
    {
        return position == startNode || position == destinationNode;
    }

    // Visualize the maze in Unity using prefabs
    void DrawMaze()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (IsStartingOrDestinationNode(new Vector2Int(x, y)))
                {
                    continue;
                }
                Vector3 position = new Vector3(x, y, 0);
                if (grid[x, y] == 1)
                {
                    // Instantiate wall
                    Instantiate(wallPrefab, position, Quaternion.identity);
                }
                else
                {
                    // Instantiate floor (optional)
                    Instantiate(floorPrefab, position, Quaternion.identity);
                }
            }
        }

        // Place the start and destination prefabs at the correct positions
        Vector3 startPosition = new Vector3(startNode.x, startNode.y, 0);
        Vector3 destinationPosition = new Vector3(destinationNode.x, destinationNode.y, 0);

        Instantiate(startPrefab, startPosition, Quaternion.identity);        // Instantiate start node prefab
        Instantiate(destinationPrefab, destinationPosition, Quaternion.identity);  // Instantiate destination node prefab
    }
}
