using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator_Prim : MonoBehaviour
{
    [HideInInspector]
    public int gridSize;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject startPrefab; // Assign a different prefab or object to indicate start
    public GameObject destinationPrefab;   // Assign a different prefab or object to indicate end

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    private int[,] grid;
    private List<Vector2Int[]> walls = new List<Vector2Int[]>();

    [HideInInspector]
    public Vector2Int startNode;
    [HideInInspector]
    public Vector2Int destinationNode;

    void Start()
    {
        gridSize = Global.GRID_SIZE;
        GenerateMaze();

        // Option 1: Random Start and End in Open Paths
        startNode = GetRandomOpenCell();
        destinationNode = GetRandomOpenCell();
        while (startNode == destinationNode) // Ensure start and end are not the same
        {
            destinationNode = GetRandomOpenCell();
        }

        // Option 2: Corners Start and End
        // Uncomment this if you prefer fixed corners as start and end
        // startNode = new Vector2Int(0, 0); // Top-left corner
        // endNode = new Vector2Int(gridSize - 1, gridSize - 1); // Bottom-right corner

        DrawMaze();
        Debug.Log($"Start Node: {startNode}, End Node: {destinationNode}");
    }

    Vector2Int GetRandomOpenCell()
    {
        Vector2Int randomCell;
        do
        {
            int x = Random.Range(0, gridSize);
            int y = Random.Range(0, gridSize);
            randomCell = new Vector2Int(x, y);
        }
        while (grid[randomCell.x, randomCell.y] != 0); // Ensure it's an open path
        return randomCell;
    }

    void GenerateMaze()
    {
        grid = new int[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = 1;
            }
        }

        Vector2Int start = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
        grid[start.x, start.y] = 0;

        foreach (var direction in directions)
        {
            Vector2Int neighbor = start + direction;
            if (IsInBounds(neighbor.x, neighbor.y))
            {
                walls.Add(new Vector2Int[] { start, neighbor });
            }
        }

        while (walls.Count > 0)
        {
            int randomIndex = Random.Range(0, walls.Count);
            Vector2Int[] wall = walls[randomIndex];
            walls.RemoveAt(randomIndex);

            Vector2Int current = wall[0];
            Vector2Int neighbor = wall[1];

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

                if (inMazeNeighbors == 1)
                {
                    grid[neighbor.x, neighbor.y] = 0;

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

    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    bool IsStartingOrDestinationNode(Vector2Int position)
    {
        return position == startNode || position == destinationNode;
    }
    
    void DrawMaze()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!IsStartingOrDestinationNode(new Vector2Int(x, y)))
                {
                    Vector3 position = new Vector3(x, y, 0);
                    if (grid[x, y] == 1)
                    {
                        Instantiate(wallPrefab, position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(floorPrefab, position, Quaternion.identity);
                    }
                }
            }
        }

        // Visualize Start and End points
        Instantiate(startPrefab, new Vector3(startNode.x, startNode.y, 0), Quaternion.identity);
        Instantiate(destinationPrefab, new Vector3(destinationNode.x, destinationNode.y, 0), Quaternion.identity);
    }
    
}
