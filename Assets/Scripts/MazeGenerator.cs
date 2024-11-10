using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [HideInInspector]
    public int gridSize;
    [HideInInspector]
    public int cellSizeInterior_x;
    [HideInInspector]
    public int cellSizeInterior_y;
    [HideInInspector]
    public int cellSizeExterior_x;
    [HideInInspector]
    public int cellSizeExterior_y;
    
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject wallPrefab;

    [Header("Map")]
    public Transform mainPath;
    public Transform optionalRooms;
    public Transform levelWalls;
    public Transform borderWalls;
    
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
    [HideInInspector]
    public Dictionary<Vector2Int, Room> allRooms;

    private List<Vector2Int> longestPath = new List<Vector2Int>();
    private HashSet<RoomPaths> allPaths;
    
    public static event Action OnMazeGenerationComplete;
    public static event Action OnAllRoomsAnalyzed;
    public static event Action OnAllRoomsPlacedHazards;
    public static event Action OnGameStart;
    public void Start()
    {
        OnAllRoomsAnalyzed += RoomsAnalyzed;
        
        gridSize = Global.GRID_SIZE;
        cellSizeInterior_x = Global.CELL_SIZE_INTERIOR_X;
        cellSizeInterior_y = Global.CELL_SIZE_INTERIOR_Y;
        cellSizeExterior_x = Global.CELL_WALL_SIZE + Global.CELL_SIZE_INTERIOR_X;
        cellSizeExterior_y = Global.CELL_WALL_SIZE + Global.CELL_SIZE_INTERIOR_Y;
        allRooms = new Dictionary<Vector2Int, Room>();
        
        StartCoroutine(SetupMaze());
    }

    public IEnumerator SetupMaze()
    {
        yield return null;
        GenerateMaze();
        GetLongestPathInMaze();
                
        yield return null;
        CreateMaze();
                
        yield return null;
        GeneratePaths();
        
        yield return null;
        GenerateEssentialAreas();

        yield return null;
        GeneratePlatforms();
                
        yield return null;
        DrawTiles();
                
        // yield return null;
        // PlacePickups();
        
        yield return null;
        AnalyzeRooms();
        
        yield return null;
        MazeGenerated();
        
        yield return new WaitForSeconds(0.5f);
        StartGame();
        
        yield return null;
        SetRoomNames(); //Primarily for in editor, can be removed in the final build
    }

    private void StartGame()
    {
        OnGameStart?.Invoke();
    }

    private void AnalyzeRooms()
    {
        IEnumerator AnalyzeRoomsAfterDelay()
        {
            // yield return new WaitForSeconds(1);
            foreach (Room room in allRooms.Values)
            {
                room.AnalyzeRoom();
                yield return null;
            }
            OnAllRoomsAnalyzed?.Invoke();
        }

        StartCoroutine(AnalyzeRoomsAfterDelay());
    }

    public void RoomsAnalyzed()
    {
        PlaceTraps();
    }
    private void PlaceTraps()
    {
        IEnumerator PlaceHazardsInRooms()
        {
            foreach (Room room in allRooms.Values)
            {
                room.PlaceHazards();
                yield return null;
            }
            OnAllRoomsPlacedHazards?.Invoke();
        }

        StartCoroutine(PlaceHazardsInRooms());
    }

    private void GenerateMaze()
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

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    private bool IsStartingNode(Vector2Int position)
    {
        return position == startNode;
    }

    private bool IsDestinationNode(Vector2Int position)
    {
        return position == destinationNode;
    }

    private bool IsMainPathNode(Vector2Int position)
    {
        return longestPath.Contains(position);
    }

    private void CreateMaze()
    {
        TilemapManager.Instance.ClearPlatformTiles();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2Int currentPosition = new Vector2Int(x, y);
                Vector3 position = new Vector3(cellSizeExterior_x * x,cellSizeExterior_y * y, 0);

                GameObject currentNodePrefab;
                Transform parentTransform;
                RoomType nodeType;
                
                if (IsStartingNode(currentPosition))
                {
                    currentNodePrefab = roomPrefab;
                    parentTransform = mainPath;
                    nodeType = RoomType.StartingRoom;
                }
                else if (IsDestinationNode(currentPosition))
                {
                    currentNodePrefab = roomPrefab;
                    parentTransform = mainPath;
                    nodeType = RoomType.DestinationRoom;
                }
                else if (IsMainPathNode(currentPosition))
                {
                    currentNodePrefab = roomPrefab;
                    parentTransform = mainPath;
                    nodeType = RoomType.MainPath;
                }
                else if (grid[x, y] == 1)
                {
                    currentNodePrefab = wallPrefab;
                    parentTransform = levelWalls;
                    nodeType = RoomType.Wall;
                }
                else
                {
                    currentNodePrefab = roomPrefab;
                    parentTransform = optionalRooms;
                    nodeType = RoomType.Optional;
                }
                GameObject spawnedNode = Instantiate(currentNodePrefab, position, Quaternion.identity, parentTransform);
                Room spawnedRoom = spawnedNode.GetComponent<Room>();
                spawnedRoom.InitRoom(nodeType, currentPosition, GlobalData.Instance.GetRoomColor(nodeType));
                allRooms.Add(currentPosition, spawnedRoom);
            }
        }
        
        /* Draw walls around the grid
        for (int x = -1; x <= gridSize; x++)
        {
            for (int y = -1; y <= gridSize; y++)
            {
                if (x == -1 || x == gridSize || y == -1 || y == gridSize)
                {
                    Vector3 wallPosition = new Vector3(cellSizeExterior_x * x, cellSizeExterior_y * y, 0);
                    GameObject spawnedNode = Instantiate(wallPrefab, wallPosition, Quaternion.identity, borderWalls);
                    Room spawnedRoom = spawnedNode.GetComponent<Room>();
                    Vector2Int currentPosition = new Vector2Int(x, y);
                    spawnedRoom.SetRoomProperties(RoomType.GridBorder, currentPosition, GlobalData.Instance.GetRoomColor(RoomType.GridBorder), wallTilemap);
                    allRooms.Add(currentPosition, spawnedRoom);
                }
            }
        }
        */
    }

    private void GetLongestPathInMaze()
    {
        Vector2Int initial = GetRandomOpenCell();
        Vector2Int farthestFromInitial = BFSFindFarthestNode(initial, out _);

        Vector2Int farthestFromFarthest = BFSFindFarthestNode(farthestFromInitial, out Dictionary<Vector2Int, Vector2Int> parents);

        longestPath.Clear();
        BacktrackLongestPath(parents, farthestFromInitial, farthestFromFarthest);

        startNode = farthestFromInitial;
        destinationNode = farthestFromFarthest;
    }

    private Vector2Int GetRandomOpenCell()
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

    private Vector2Int BFSFindFarthestNode(Vector2Int start, out Dictionary<Vector2Int, Vector2Int> parents)
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

    private void BacktrackLongestPath(Dictionary<Vector2Int, Vector2Int> parents, Vector2Int start, Vector2Int end)
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

    private void MazeGenerated()
    {
        OnMazeGenerationComplete?.Invoke();
    }

    private void GeneratePaths()
    {
        allPaths = new HashSet<RoomPaths>();
        foreach (Vector2Int gridIndex in allRooms.Keys)
        {
            Room thisRoom = allRooms[gridIndex];
            if (!IsVisitableRoom(thisRoom))
            {
                continue;
            }
            foreach (RoomEdge edge in Global.Directions.Keys)
            {
                Vector2Int neighbor = gridIndex + Global.Directions[edge];
                RoomPaths pathA = new RoomPaths(gridIndex, neighbor);
                RoomPaths pathB = new RoomPaths(neighbor, gridIndex);
                if (allPaths.Contains(pathA) || allPaths.Contains(pathB))
                {
                    //this path has already been created
                    continue;
                }

                if (allRooms.TryGetValue(neighbor, out var neighborRoom))
                {
                    if (IsVisitableRoom(neighborRoom))
                    {
                        allPaths.Add(pathA);
                        int[] path;
                        switch (edge)
                        {
                            case RoomEdge.Ceiling:
                                //found a neighbor above this cell, so choose a horizontal border and open the corresponding one in both rooms
                                path = GetPathIndices(RoomEdge.Ceiling);
                                thisRoom.CreateOpening(RoomEdge.Ceiling, path, neighborRoom);
                                neighborRoom.CreateOpening(RoomEdge.Floor, path, thisRoom);
                                break;
                            
                            case RoomEdge.Floor:
                                path = GetPathIndices(RoomEdge.Floor);
                                thisRoom.CreateOpening(RoomEdge.Floor, path, neighborRoom);
                                neighborRoom.CreateOpening(RoomEdge.Ceiling, path, thisRoom);
                                break;
                            case RoomEdge.Left:
                                path = GetPathIndices(RoomEdge.Left);
                                thisRoom.CreateOpening(RoomEdge.Left, path, neighborRoom);
                                neighborRoom.CreateOpening(RoomEdge.Right, path, thisRoom);
                                break;
                            case RoomEdge.Right:
                                path = GetPathIndices(RoomEdge.Right);
                                thisRoom.CreateOpening(RoomEdge.Right, path, neighborRoom);
                                neighborRoom.CreateOpening(RoomEdge.Left, path, thisRoom);
                                break;
                        }
                    }
                }
            }
        }
    }
    
    private void GenerateEssentialAreas()
    {
        allRooms[startNode].gameObject.AddComponent<StartingRoom>().SetupStartingRoom(allRooms[startNode], allRooms[startNode].platformGenerator);
    }

    private void GeneratePlatforms()
    {
        foreach (Room room in allRooms.Values)
        {
            room.GeneratePlatforms();
        }
    }

    private void DrawTiles()
    {
        foreach (Room room in allRooms.Values)
        {
            room.PlaceBorderTiles();
            room.DrawOpening();
        }
    }

    private void SetRoomNames()
    {
        int roomNumber = 0;
        foreach (Vector2Int gridPosition in allRooms.Keys)
        {
            string suffix = "";
            if (longestPath.Contains(gridPosition))
            {
                roomNumber++;
                suffix = "Main Path|" + roomNumber;
            }
            Room room = allRooms[gridPosition];
            room.gameObject.name = gridPosition.ToString() + suffix;
        }
        foreach (Vector2Int gridIndex in longestPath)
        {
        }
    }

    public bool IsVisitableRoom(Room room)
    {
        return room.roomType != RoomType.Wall && room.roomType != RoomType.GridBorder;
    }
    
    private void PlacePickups()
    {
        // allRooms[startNode].SpawnPickupInSector(PickupType.MultiJump);
        // allRooms[startNode].SpawnPickupInSector(PickupType.WallGrab);
    }

    private int[]GetPathIndices(RoomEdge edge)
    {
        bool isAWall = edge == RoomEdge.Left || edge == RoomEdge.Right;
        int rangeLength = Random.Range(Global.MIN_PATH_WIDTH, Global.MAX_PATH_WIDTH + 1);
        int start = Random.Range(0, (isAWall ? Global.CELL_SIZE_INTERIOR_Y : Global.CELL_SIZE_INTERIOR_X) - rangeLength + 1);
        int[] result = new int[rangeLength];
        for (int i = 0; i < rangeLength; i++)
        {
            result[i] = start + i;
        }

        return result;
    }
}
