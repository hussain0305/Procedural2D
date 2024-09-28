using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prim_MazeGeneration : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject startPrefab;        // Prefab for start node
    public GameObject destinationPrefab;  // Prefab for destination node
    public int minPathLength = 10;        // Minimum path length

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0), // Left
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(0, 1)   // Up
    };

    private int[,] grid; // 1 for wall, 0 for path
    private List<Vector2Int[]> walls = new List<Vector2Int[]>(); // List of walls to be processed
    private Vector2Int startNode;
    private Vector2Int destinationNode;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        grid = new int[Global.GRID_SIZE, Global.GRID_SIZE];

        // Initialize the grid with walls
        for (int x = 0; x < Global.GRID_SIZE; x++)
        {
            for (int y = 0; y < Global.GRID_SIZE; y++)
            {
                grid[x, y] = 1; // 1 means a wall
            }
        }

        // Set start and destination nodes
        startNode = new Vector2Int(0, 0);  // Top-left corner
        destinationNode = new Vector2Int(Global.GRID_SIZE - 1, Global.GRID_SIZE - 1); // Bottom-right corner

        // Step 1: Create a guaranteed path between start and destination nodes with the minimum length
        CreateGuaranteedPath();

        // Step 2: Use Prim's algorithm to generate the rest of the maze
        PrimAlgorithm();
    }

    // Step 1: Create a path from start to destination that guarantees the minimum length
    void CreateGuaranteedPath()
    {
        Vector2Int current = startNode;
        int currentPathLength = 0;
        grid[current.x, current.y] = 0; // Mark start node as part of the path

        while (current != destinationNode && currentPathLength < minPathLength)
        {
            Vector2Int nextStep = GetNextStep(current);
            if (nextStep != current)
            {
                grid[nextStep.x, nextStep.y] = 0; // Mark as path
                current = nextStep;
                currentPathLength++;
            }
        }

        // Ensure the destination node is connected to the path
        grid[destinationNode.x, destinationNode.y] = 0;
    }

    // Get the next step that moves toward the destination node and stretches the path
    Vector2Int GetNextStep(Vector2Int current)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int next = current + dir;
            if (IsInBounds(next.x, next.y) && grid[next.x, next.y] == 1)
            {
                possibleMoves.Add(next);
            }
        }

        // Prioritize moving toward the destination node
        if (possibleMoves.Count > 0)
        {
            possibleMoves.Sort((a, b) => ManhattanDistance(a, destinationNode).CompareTo(ManhattanDistance(b, destinationNode)));
            return possibleMoves[0]; // Move closest to the destination
        }
        return current; // No move available
    }

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Step 2: Run Prim's algorithm for the rest of the maze
    void PrimAlgorithm()
    {
        // Add surrounding walls of the start node to the list
        foreach (var direction in directions)
        {
            Vector2Int neighbor = startNode + direction;
            if (IsInBounds(neighbor.x, neighbor.y) && grid[neighbor.x, neighbor.y] == 1)
            {
                walls.Add(new Vector2Int[] { startNode, neighbor });
            }
        }

        // Process walls
        while (walls.Count > 0)
        {
            // Pick a random wall
            int randomIndex = Random.Range(0, walls.Count);
            Vector2Int[] wall = walls[randomIndex];
            walls.RemoveAt(randomIndex);
            Vector2Int current = wall[0]; // Current cell
            Vector2Int neighbor = wall[1]; // Neighboring cell

            if (grid[neighbor.x, neighbor.y] == 1)
            {
                int inMazeNeighbors = 0;
                foreach (var direction in directions)
                {
                    Vector2Int adjacent = neighbor + direction;
                    if (IsInBounds(adjacent.x, adjacent.y) && grid[adjacent.x, adjacent.y] == 0)
                    {
                        inMazeNeighbors++;
                    }
                }

                // If the neighboring cell has only one connection to the maze, carve it out
                if (inMazeNeighbors == 1)
                {
                    grid[neighbor.x, neighbor.y] = 0;

                    // Add neighboring walls to the list
                    foreach (var direction in directions)
                    {
                        Vector2Int adjacent = neighbor + direction;
                        if (IsInBounds(adjacent.x, adjacent.y) && grid[adjacent.x, adjacent.y] == 1)
                        {
                            walls.Add(new Vector2Int[] { neighbor, adjacent });
                        }
                    }
                }
            }
        }
    }

    // Helper function to check if a cell is within grid bounds
    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Global.GRID_SIZE && y >= 0 && y < Global.GRID_SIZE;
    }

    bool IsStartingOrDestinationNode(Vector2Int position)
    {
        return position == startNode || position == destinationNode;
    }

    // Visualize the maze in Unity using prefabs
    void DrawMaze()
    {
        for (int x = 0; x < Global.GRID_SIZE; x++)
        {
            for (int y = 0; y < Global.GRID_SIZE; y++)
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

        // Instantiate start and destination prefabs
        Vector3 startPosition = new Vector3(startNode.x, startNode.y, 0);
        Vector3 destinationPosition = new Vector3(destinationNode.x, destinationNode.y, 0);

        Instantiate(startPrefab, startPosition, Quaternion.identity);        // Instantiate start node prefab
        Instantiate(destinationPrefab, destinationPosition, Quaternion.identity);  // Instantiate destination node prefab
    }
}
