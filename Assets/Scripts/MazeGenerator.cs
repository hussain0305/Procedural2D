using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MazeGenerator : MonoBehaviour
{
    [HideInInspector]
    public int gridSize;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject startPrefab;
    public GameObject destinationPrefab;
    public GameObject mainPathPrefab;

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

    private List<Vector2Int> longestPath = new List<Vector2Int>();

    void Start()
    {
        gridSize = Global.GRID_SIZE;
        GenerateMaze();

        (startNode, destinationNode, longestPath) = GetLongestPathInMaze();

        DrawMaze();
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

    bool IsStartingNode(Vector2Int position)
    {
        return position == startNode;
    }

    bool IsDestinationNode(Vector2Int position)
    {
        return position == destinationNode;
    }

    bool IsMainPathNode(Vector2Int position)
    {
        return longestPath.Contains(position);
    }

    void DrawMaze()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2Int currentPosition = new Vector2Int(x, y);
                Vector3 position = new Vector3(x, y, 0);

                GameObject currentNodePrefab;
                if (IsStartingNode(currentPosition))
                {
                    currentNodePrefab = startPrefab;
                }
                else if (IsDestinationNode(currentPosition))
                {
                    currentNodePrefab = destinationPrefab;
                }
                else if (IsMainPathNode(currentPosition))
                {
                    currentNodePrefab = mainPathPrefab;
                }
                else if (grid[x, y] == 1)
                {
                    currentNodePrefab = wallPrefab;
                }
                else
                {
                    currentNodePrefab = floorPrefab;
                }
                Instantiate(currentNodePrefab, position, Quaternion.identity);
            }
        }
    }

    (Vector2Int, Vector2Int, List<Vector2Int>) GetLongestPathInMaze()
    {
        Vector2Int initial = GetRandomOpenCell();
        Vector2Int farthestFromInitial = BFSFindFarthestNode(initial, out _);

        Vector2Int farthestFromFarthest = BFSFindFarthestNode(farthestFromInitial, out Dictionary<Vector2Int, Vector2Int> parents);

        longestPath.Clear();
        BacktrackLongestPath(parents, farthestFromInitial, farthestFromFarthest);

        return (farthestFromInitial, farthestFromFarthest, longestPath);
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
        while (grid[randomCell.x, randomCell.y] != 0);
        return randomCell;
    }

    Vector2Int BFSFindFarthestNode(Vector2Int start, out Dictionary<Vector2Int, Vector2Int> parents)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        parents = new Dictionary<Vector2Int, Vector2Int>();
        bool[,] visited = new bool[gridSize, gridSize];
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        Vector2Int farthestNode = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            farthestNode = current;

            foreach (var direction in directions)
            {
                Vector2Int neighbor = current + direction;
                if (IsInBounds(neighbor.x, neighbor.y) && grid[neighbor.x, neighbor.y] == 0 && !visited[neighbor.x, neighbor.y])
                {
                    visited[neighbor.x, neighbor.y] = true;
                    parents[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return farthestNode;
    }

    void BacktrackLongestPath(Dictionary<Vector2Int, Vector2Int> parents, Vector2Int start, Vector2Int end)
    {
        longestPath.Clear();
        Vector2Int current = end;

        while (current != start)
        {
            longestPath.Add(current);
            if (!parents.TryGetValue(current, out current))
            {
                break;
            }
        }
        longestPath.Add(start);
        longestPath.Reverse();
    }
}
