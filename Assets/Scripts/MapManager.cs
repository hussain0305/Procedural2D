using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public GameObject roomPrefab;

    public Transform levelMap;
    
    private MapProperties map;
    private static Dictionary<Sector, List<Sector>> adjacentSectorsDictionary;
    private static Dictionary<Sector, List<Sector>> destinationSectorsDictionary;
    private void Start()
    {
        InitializeAdjacentSectors();
        InitializeDestinationSectors();
        GenerateMap();
    }

    private void GenerateMap()
    {
        map.mainPathRooms = new Dictionary<int, Room>();
        map.numRooms_MainPath = Random.Range(Global.MIN_ROOMS_IN_MAIN_PATH, Global.MAX_ROOMS_IN_MAIN_PATH + 1);
        Sector startingSector = GetRandomSector();
        Vector2Int startingRoomPosition = GetRandomRoomInSector(startingSector);
        map.startingRoom = CreateStartRoomAt(startingRoomPosition);
        Debug.Log("Starting Sector is " + startingSector + " and room position is " + startingRoomPosition );
        Sector startingRoomSector = GetRoomSector(map.startingRoom);
        Debug.Log("Fetched Starting Sector is " + startingRoomSector);
        
        List<Sector> possibleDestinations = (GetDestinationSectors(startingRoomSector));
        Sector destinationSector = possibleDestinations[Random.Range(0, possibleDestinations.Count)];
        Vector2Int destinationRoomPosition = GetRandomRoomInSector(destinationSector);
        Debug.Log("Destination Sector is " + destinationSector + " and room position is " + destinationRoomPosition );
        map.destinationRoom = CreateDestinationRoomAt(destinationRoomPosition);

        List<Vector2Int> path = GeneratePathWithObstacles(startingRoomPosition, destinationRoomPosition, map.numRooms_MainPath, Global.GRID_SIZE);
        int i = 0;
        foreach (Vector2Int pathRoom in path)
        {
            if (pathRoom != startingRoomPosition && pathRoom != destinationRoomPosition)
            {
                Room mainPathRoom = CreateRoomAt(pathRoom);
                map.mainPathRooms.Add(i, mainPathRoom);
            }
            i++;
        }
    }
    
    Sector GetRandomSector()
    {
        Sector[] sectors = (Sector[])System.Enum.GetValues(typeof(Sector));
        return sectors[Random.Range(0, sectors.Length)];
    }

    public Sector GetRoomSector(Room room)
    {
        return GetRoomSector(room.position);
    }
    
    public Sector GetRoomSector(Vector2Int room)
    {
        int x = room.x;
        int y = room.y;

        const int OUTER_SECTOR_BORDER = Global.GRID_SIZE;
        const int LOWER_SECTOR_BORDER = Global.GRID_SIZE / 3;
        const int UPPER_SECTOR_BORDER = 2 * LOWER_SECTOR_BORDER;
        
        if (x < LOWER_SECTOR_BORDER && y < LOWER_SECTOR_BORDER) return Sector.LeftBottom;
        if (x < LOWER_SECTOR_BORDER && y < UPPER_SECTOR_BORDER) return Sector.LeftMid;
        if (x < LOWER_SECTOR_BORDER && y < OUTER_SECTOR_BORDER) return Sector.LeftTop;

        if (x < UPPER_SECTOR_BORDER && y < LOWER_SECTOR_BORDER) return Sector.CenterBottom;
        if (x < UPPER_SECTOR_BORDER && y < UPPER_SECTOR_BORDER) return Sector.CenterMid;
        if (x < UPPER_SECTOR_BORDER && y < OUTER_SECTOR_BORDER) return Sector.CenterTop;

        if (x < OUTER_SECTOR_BORDER && y < LOWER_SECTOR_BORDER) return Sector.RightBottom;
        if (x < OUTER_SECTOR_BORDER && y < UPPER_SECTOR_BORDER) return Sector.RightMid;
        if (x < OUTER_SECTOR_BORDER && y < OUTER_SECTOR_BORDER) return Sector.RightTop;

        return Sector.CenterMid;
    }

    List<Sector> GetAdjacentSectors(Sector sector)
    {
        if (adjacentSectorsDictionary == null)
        {
            InitializeAdjacentSectors();
        }
        return adjacentSectorsDictionary[sector];
    }
    
    List<Sector> GetDestinationSectors(Sector sector)
    {
        if (destinationSectorsDictionary == null)
        {
            InitializeDestinationSectors();
        }
        return destinationSectorsDictionary[sector];
    }

    Vector2Int GetRandomRoomInSector(Sector sector)
    {
        int xOffset = 0, yOffset = 0;
        
        const int INNER_SECTOR_BORDER = 0;
        const int OUTER_SECTOR_BORDER = Global.GRID_SIZE;
        const int LOWER_SECTOR_BORDER = Global.GRID_SIZE / 3;
        const int UPPER_SECTOR_BORDER = 2 * LOWER_SECTOR_BORDER;

        int x = Random.Range(0, LOWER_SECTOR_BORDER);
        int y = Random.Range(0, LOWER_SECTOR_BORDER);

        switch (sector)
        {
            case Sector.LeftBottom:
                xOffset = INNER_SECTOR_BORDER;
                yOffset = INNER_SECTOR_BORDER;
                break;
            case Sector.CenterBottom:
                xOffset = LOWER_SECTOR_BORDER;
                yOffset = INNER_SECTOR_BORDER;
                break;
            case Sector.RightBottom:
                xOffset = UPPER_SECTOR_BORDER;
                yOffset = INNER_SECTOR_BORDER;
                break;
            case Sector.LeftMid:
                xOffset = INNER_SECTOR_BORDER;
                yOffset = LOWER_SECTOR_BORDER;
                break;
            case Sector.CenterMid:
                xOffset = LOWER_SECTOR_BORDER;
                yOffset = LOWER_SECTOR_BORDER;
                break;
            case Sector.RightMid:
                xOffset = UPPER_SECTOR_BORDER;
                yOffset = LOWER_SECTOR_BORDER;
                break;
            case Sector.LeftTop:
                xOffset = INNER_SECTOR_BORDER;
                yOffset = UPPER_SECTOR_BORDER;
                break;
            case Sector.CenterTop:
                xOffset = LOWER_SECTOR_BORDER;
                yOffset = UPPER_SECTOR_BORDER;
                break;
            case Sector.RightTop:
                xOffset = UPPER_SECTOR_BORDER;
                yOffset = UPPER_SECTOR_BORDER;
                break;
            default:
                xOffset = yOffset = 0; // Fallback case
                break;
        }

        // Add the offsets to the random x and y
        return new Vector2Int(x + xOffset, y + yOffset);
    }

    public Room CreateStartRoomAt(Vector2Int RoomLocation)
    {
        Room room = CreateRoomAt(RoomLocation);
        room.roomSprite.color = Color.green;
        return room;
    }
    
    public Room CreateDestinationRoomAt(Vector2Int RoomLocation)
    {
        Room room = CreateRoomAt(RoomLocation);
        room.roomSprite.color = Color.red;
        return room;
    }
    
    public Room CreateRoomAt(Vector2Int RoomLocation)
    {
        GameObject spawnedGO = Instantiate(roomPrefab, new Vector2(RoomLocation.x, RoomLocation.y), Quaternion.identity, levelMap);
        Room spawnedRoom = spawnedGO.GetComponent<Room>();
        spawnedRoom.position = RoomLocation;
        return spawnedRoom;
    }
    
    List<Vector2Int> GeneratePathWithObstacles(Vector2Int start, Vector2Int end, int minRooms, int gridSize)
    {
        HashSet<Vector2Int> fakeObstacles = new HashSet<Vector2Int>();
        List<Vector2Int> path = AStarPathfinding(start, end, gridSize, fakeObstacles);

        // Keep adding fake obstacles until the path length meets the minimum rooms requirement
        while (path.Count < minRooms)
        {
            // Pick a random room to be a "fake" obstacle, avoiding the start and end rooms
            Vector2Int randomRoom = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));

            if (!path.Contains(randomRoom) && randomRoom != start && randomRoom != end)
            {
                fakeObstacles.Add(randomRoom);
            }

            // Recalculate the path with the new fake obstacles
            path = AStarPathfinding(start, end, gridSize, fakeObstacles);
        }

        return path;
    }

    List<Vector2Int> AStarPathfinding(Vector2Int start, Vector2Int goal, int gridSize, HashSet<Vector2Int> obstacles)
{
    Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
    Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();

    PriorityQueue<Vector2Int, int> frontier = new PriorityQueue<Vector2Int, int>();
    frontier.Enqueue(start, 0);

    cameFrom[start] = start;
    costSoFar[start] = 0;

    while (frontier.Count > 0)
    {
        Vector2Int current = frontier.Dequeue();

        if (current == goal)
            break;

        foreach (var next in GetNeighbors(current, gridSize))
        {
            if (obstacles.Contains(next)) // Skip rooms with obstacles
                continue;

            int newCost = costSoFar[current] + 1;

            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
            {
                costSoFar[next] = newCost;
                int priority = newCost + Heuristic(goal, next);
                frontier.Enqueue(next, priority);
                cameFrom[next] = next;
            }
        }
    }

    return ReconstructPath(cameFrom, start, goal);
    }

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    List<Vector2Int> GetNeighbors(Vector2Int room, int gridSize)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Add valid neighboring rooms
        if (room.x + 1 < gridSize) neighbors.Add(new Vector2Int(room.x + 1, room.y));
        if (room.x - 1 >= 0) neighbors.Add(new Vector2Int(room.x - 1, room.y));
        if (room.y + 1 < gridSize) neighbors.Add(new Vector2Int(room.x, room.y + 1));
        if (room.y - 1 >= 0) neighbors.Add(new Vector2Int(room.x, room.y - 1));

        return neighbors;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = goal;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }


#region Initializations
    void InitializeAdjacentSectors()
    { 
        List<Sector> GetAdjacentSectorsForInitialization(Sector sector)
        {
            List<Sector> adjacentSectors = new List<Sector>();

            switch (sector)
            {
                case Sector.LeftTop:
                    adjacentSectors.Add(Sector.CenterTop);
                    adjacentSectors.Add(Sector.LeftMid);
                    break;
                case Sector.CenterTop:
                    adjacentSectors.Add(Sector.LeftTop);
                    adjacentSectors.Add(Sector.RightTop);
                    adjacentSectors.Add(Sector.CenterMid);
                    break;
                case Sector.RightTop:
                    adjacentSectors.Add(Sector.CenterTop);
                    adjacentSectors.Add(Sector.RightMid);
                    break;
                case Sector.LeftMid:
                    adjacentSectors.Add(Sector.LeftTop);
                    adjacentSectors.Add(Sector.CenterMid);
                    adjacentSectors.Add(Sector.LeftBottom);
                    break;
                case Sector.CenterMid:
                    adjacentSectors.Add(Sector.CenterTop);
                    adjacentSectors.Add(Sector.LeftMid);
                    adjacentSectors.Add(Sector.RightMid);
                    adjacentSectors.Add(Sector.CenterBottom);
                    break;
                case Sector.RightMid:
                    adjacentSectors.Add(Sector.RightTop);
                    adjacentSectors.Add(Sector.CenterMid);
                    adjacentSectors.Add(Sector.RightBottom);
                    break;
                case Sector.LeftBottom:
                    adjacentSectors.Add(Sector.LeftMid);
                    adjacentSectors.Add(Sector.CenterBottom);
                    break;
                case Sector.CenterBottom:
                    adjacentSectors.Add(Sector.LeftBottom);
                    adjacentSectors.Add(Sector.RightBottom);
                    adjacentSectors.Add(Sector.CenterMid);
                    break;
                case Sector.RightBottom:
                    adjacentSectors.Add(Sector.RightMid);
                    adjacentSectors.Add(Sector.CenterBottom);
                    break;
            }
            return adjacentSectors;
        }

        adjacentSectorsDictionary = new Dictionary<Sector, List<Sector>>();
        foreach (Sector sector in System.Enum.GetValues(typeof(Sector)))
        {
            adjacentSectorsDictionary[sector] = GetAdjacentSectorsForInitialization(sector);
        }
    }

    void InitializeDestinationSectors()
    {
        destinationSectorsDictionary = new Dictionary<Sector, List<Sector>>();

        foreach (Sector sector in System.Enum.GetValues(typeof(Sector)))
        {
            List<Sector> adjacentSectors = GetAdjacentSectors(sector);
            List<Sector> eligibleSectors = new List<Sector>();

            foreach (Sector s in System.Enum.GetValues(typeof(Sector)))
            {
                if (!adjacentSectors.Contains(s) && s != sector)
                {
                    eligibleSectors.Add(s);
                }
            }

            destinationSectorsDictionary[sector] = eligibleSectors;
        }
    }
#endregion

}
