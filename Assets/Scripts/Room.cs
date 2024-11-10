using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform roomInterior;
    public Transform roomContentsSimulateConditionally;
    public Transform roomContentsSimulateAlways;
    public Transform topLeftCorner;
    public Transform topRightCorner;
    public Transform bottomLeftCorner;
    public Transform bottomRightCorner;
    public RoomBorder leftWall;
    public RoomBorder rightWall;
    public RoomBorder ceiling;
    public RoomBorder floor;

    public SpriteRenderer roomSprite;

    public RoomPlatformGenerator platformGenerator;
    public RoomAnalyzer roomAnalyzer;
    
    [HideInInspector]
    public Vector2Int gridIndex;

    [HideInInspector]
    public RoomType roomType;

    [HideInInspector]
    public int roomNumber;

    [HideInInspector]
    public RoomInterior roomVolume;

    [HideInInspector]
    public RoomSetupProgress setupProgress;

    [HideInInspector]
    public bool[,] roomGridBuildings;

    [HideInInspector]
    public AreaType[,] roomGridNPCAreas;

    [HideInInspector]
    public List<Vector2Int> pathwayCells = new List<Vector2Int>();

    private Dictionary<Room, RoomOpening> neighbours = new Dictionary<Room, RoomOpening>();
    
    private void Awake()
    {
        SetScaleAndBorders();
    }

    private void OnEnable()
    {
        EventManager.OnRoomEntered += EvaluateRoomSimulation;
    }

    private void OnDisable()
    {
        EventManager.OnRoomEntered -= EvaluateRoomSimulation;
    }

    public void EvaluateRoomSimulation(Vector2Int playerCellPosition)
    {
        roomContentsSimulateConditionally?.gameObject.SetActive(GameManager.Instance.ShouldSimulate(gridIndex, playerCellPosition));
    }
    
    private void SetScaleAndBorders()
    {
        roomSprite.transform.localScale = new Vector3(Global.CELL_SIZE_INTERIOR_X + Global.CELL_WALL_SIZE, Global.CELL_SIZE_INTERIOR_Y + Global.CELL_WALL_SIZE, 1);
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + leftWall.transform.localScale.x);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + leftWall.transform.localScale.x);

        Vector2Int leftDirection = Global.GetDirection(RoomEdge.Left);
        leftWall.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * leftDirection.x, edgeDistanceFromCenter_y * leftDirection.y);
        
        Vector2Int rightDirection = Global.GetDirection(RoomEdge.Right);
        rightWall.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * rightDirection.x, edgeDistanceFromCenter_y * rightDirection.y);

        Vector2Int ceilingDirection = Global.GetDirection(RoomEdge.Ceiling);
        ceiling.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * ceilingDirection.x, edgeDistanceFromCenter_y * ceilingDirection.y);

        Vector2Int floorDirection = Global.GetDirection(RoomEdge.Floor);
        floor.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * floorDirection.x, edgeDistanceFromCenter_y * floorDirection.y);
        
        topLeftCorner.transform.localPosition = new Vector3(-1 * edgeDistanceFromCenter_x, edgeDistanceFromCenter_y);
        topRightCorner.transform.localPosition = new Vector3(edgeDistanceFromCenter_x, edgeDistanceFromCenter_y);
        bottomLeftCorner.transform.localPosition = new Vector3(-1 * edgeDistanceFromCenter_x, -1 * edgeDistanceFromCenter_y);
        bottomRightCorner.transform.localPosition = new Vector3(edgeDistanceFromCenter_x, -1 * edgeDistanceFromCenter_y);
        
        leftWall.SetupLeftBorder();
        rightWall.SetupRightBorder();
        ceiling.SetupCeiling();
        floor.SetupFloor();
        setupProgress.SetBorderBlocksPlacementCompleted();
    }

    public void PlaceBorderTiles()
    {
        Vector2Int roomWorldPosition = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
        int roomHalfWidth = (int)Global.CELL_SIZE_INTERIOR_X / 2;
        int roomHalfHeight = (int)Global.CELL_SIZE_INTERIOR_Y / 2;
            
        Vector2Int topLeftGridPosition = new Vector2Int(-roomHalfWidth - 1, roomHalfHeight);
        Vector2Int bottomRightGridPosition = new Vector2Int(roomHalfWidth, -roomHalfHeight - 1);
        Vector2Int topRightGridPosition = new Vector2Int(roomHalfWidth, roomHalfHeight);
        Vector2Int bottomLeftGridPosition = new Vector2Int(-roomHalfWidth - 1, -roomHalfHeight - 1);
        Vector3Int tilePosition;
            
        for (int x = topLeftGridPosition.x; x <= topRightGridPosition.x; x++)
        {
            tilePosition = new Vector3Int(x + roomWorldPosition.x, topLeftGridPosition.y + roomWorldPosition.y, 0);
            TilemapManager.Instance.PlaceWallTileAt(tilePosition);
            tilePosition = new Vector3Int(x + roomWorldPosition.x, bottomLeftGridPosition.y + roomWorldPosition.y, 0);
            TilemapManager.Instance.PlaceWallTileAt(tilePosition);
        }
        for (int y = bottomLeftGridPosition.y; y <= topLeftGridPosition.y; y++)
        {
            tilePosition = new Vector3Int(bottomLeftGridPosition.x + roomWorldPosition.x, y + roomWorldPosition.y, 0);
            TilemapManager.Instance.PlaceWallTileAt(tilePosition);
            tilePosition = new Vector3Int(topRightGridPosition.x + roomWorldPosition.x, y + roomWorldPosition.y, 0);
            TilemapManager.Instance.PlaceWallTileAt(tilePosition);
        }

        setupProgress.SetBorderTilesPlacementCompleted();
    }
    
    public void InitRoom(RoomType _roomType, Vector2Int _gridIndex, Material _material)
    {
        roomType = _roomType;
        gridIndex = _gridIndex;
        roomSprite.sharedMaterial = _material;
        
        roomVolume = GetComponentInChildren<RoomInterior>();
        roomVolume.gridIndex = gridIndex;
        
        roomGridBuildings = new bool[Global.CELL_SIZE_INTERIOR_X, Global.CELL_SIZE_INTERIOR_Y];
        roomGridNPCAreas = new AreaType[Global.CELL_SIZE_INTERIOR_X, Global.CELL_SIZE_INTERIOR_Y];
        
        platformGenerator?.Init(pathwayCells);
    }

    public void CreateOpening(RoomEdge _edge, int[] path, Room neighbour)
    {
        ClearPathway(_edge, path);
        RoomOpening opening = new RoomOpening(_edge, path, new List<Vector3Int>());
        foreach (RoomBorder roomBorder in GetComponentsInChildren<RoomBorder>())
        {
            if (roomBorder.edge == _edge)
            {
                for (int i = path[0]; i <= path[path.Length - 1]; i++)
                {
                    if (roomBorder.borderBlock[i] != null)
                    {
                        opening.blockPositions.Add(new Vector3Int((int)(roomBorder.borderBlock[i].transform.position.x - 0.5f), (int)(roomBorder.borderBlock[i].transform.position.y - 0.5f), 0));
                        Destroy(roomBorder.borderBlock[i]);
                        roomBorder.borderBlock[i] = null;
                    }
                }
                neighbours.Add(neighbour, opening);
                break;
            }
        }
        setupProgress.SetPathCreationCompleted();
    }

    private void ClearPathway(RoomEdge edge, int[] path)
    {
        switch (edge)
        {
            case RoomEdge.Left:
                foreach (int columnIndex in path)
                {
                    for (int i = 0; i < Global.PATHWAY_WIDTH; i++)
                    {
                        pathwayCells.Add(new Vector2Int(i, columnIndex));
                    }
                }
                break;

            case RoomEdge.Right:
                foreach (int columnIndex in path)
                {
                    for (int i = Global.CELL_SIZE_INTERIOR_X - 1; i >= Global.CELL_SIZE_INTERIOR_X - Global.PATHWAY_WIDTH; i--)
                    {
                        pathwayCells.Add(new Vector2Int(i, columnIndex));
                    }
                }
                break;
            case RoomEdge.Ceiling:
                foreach (int rowIndex in path)
                {
                    for (int i = Global.CELL_SIZE_INTERIOR_Y - 1; i >= Global.CELL_SIZE_INTERIOR_Y - Global.PATHWAY_WIDTH; i--)
                    {
                        pathwayCells.Add(new Vector2Int(rowIndex, i));
                    }
                }
                break;
            case RoomEdge.Floor:
                foreach (int rowIndex in path)
                {
                    for (int i = 0; i < Global.PATHWAY_WIDTH; i++)
                    {
                        pathwayCells.Add(new Vector2Int(rowIndex, i));
                    }
                }
                break;
        }
    }

    public void DrawOpening()
    {
        foreach (RoomOpening opening in neighbours.Values)
        {
            foreach (Vector3Int blockPosition in opening.blockPositions)
            {
                TilemapManager.Instance.RemoveWallTileFrom(blockPosition);
            }
        }
        setupProgress.SetPathTilesRemovalCompleted();
    }

    public void GeneratePlatforms()
    {
        platformGenerator?.GeneratePlatforms();
    }
    
    public void SpawnPickupInSector(PickupType pickup)
    {
        GameObject pickupPrefab = GlobalData.Instance.GetPickupPrefab(pickup);
        if (pickupPrefab)
        {
            Instantiate(pickupPrefab, transform.position - new Vector3(4, 4, 0), Quaternion.identity, transform);
        }
    }

    public void AnalyzeRoom()
    {
        roomAnalyzer?.AnalyzeRoom(roomGridBuildings);
    }

    public List<List<Vector2Int>> GetSpaciousAreas()
    {
        return roomAnalyzer.spaciousAreas;
    }

    public List<Vector2Int> GetBiggestSpaciousArea()
    {
        if (roomAnalyzer.spaciousAreas == null || roomAnalyzer.spaciousAreas.Count == 0)
        {
            return null;
        }

        List<Vector2Int> biggestSpaciousArea = roomAnalyzer.spaciousAreas[0];
        foreach (List<Vector2Int> spaciousArea in roomAnalyzer.spaciousAreas)
        {
            if (spaciousArea.Count > biggestSpaciousArea.Count)
            {
                biggestSpaciousArea = spaciousArea;
            }
        }

        return biggestSpaciousArea;
    }

    public List<Vector2Int> GetBiggestSpaciousAreaOutsideOfPathways()
    {
        if (roomAnalyzer.spaciousAreas == null || roomAnalyzer.spaciousAreas.Count == 0)
        {
            return null;
        }

        List<Vector2Int> biggestSpaciousArea = roomAnalyzer.spaciousAreas[0];
        foreach (List<Vector2Int> spaciousArea in roomAnalyzer.spaciousAreas)
        {
            if (spaciousArea.Count > biggestSpaciousArea.Count && !AreaIsAPathway(spaciousArea))
            {
                biggestSpaciousArea = spaciousArea;
            }
        }

        return biggestSpaciousArea;
    }

    public Vector2Int GetCenterOfArea(List<Vector2Int> area)
    {
        if (area == null || area.Count == 0)
        {
            Debug.LogError("Area list is empty or null.");
            return Vector2Int.zero;
        }

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (Vector2Int point in area)
        {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        int centerX = (minX + maxX) / 2;
        int centerY = (minY + maxY) / 2;

        List<Vector2Int> centerPoints = new List<Vector2Int>();
        foreach (Vector2Int point in area)
        {
            if (point.x == centerX || point.y == centerY)
            {
                centerPoints.Add(point);
            }
        }

        if (centerPoints.Count > 0)
        {
            centerPoints.Sort((a, b) => a.x.CompareTo(b.x));
            int middleIndex = centerPoints.Count / 2;
            return centerPoints[middleIndex];
        }

        return new Vector2Int(centerX, centerY);
    }

    public Vector2Int GetBottomCenterOfArea(List<Vector2Int> area)
    {
        if (area == null || area.Count == 0)
        {
            Debug.LogError("Area list is empty or null.");
            return Vector2Int.zero;
        }

        int minY = int.MaxValue;
        foreach (Vector2Int point in area)
        {
            if (point.y < minY)
            {
                minY = point.y;
            }
        }

        List<Vector2Int> bottomRowPoints = new List<Vector2Int>();
        foreach (Vector2Int point in area)
        {
            if (point.y == minY)
            {
                bottomRowPoints.Add(point);
            }
        }

        bottomRowPoints.Sort((a, b) => a.x.CompareTo(b.x));

        int middleIndex = bottomRowPoints.Count / 2;

        return bottomRowPoints[middleIndex];
    }

    public Vector3 GetCellLocation(Vector2Int cell)
    {
        return transform.position + new Vector3(cell.x - ((float)Global.CELL_SIZE_INTERIOR_X / 2), cell.y - ((float)Global.CELL_SIZE_INTERIOR_Y / 2), 0);
    }
    
    public bool AreaIsAPathway(List<Vector2Int> area)
    {
        foreach (Vector2Int cell in area)
        {
            if (pathwayCells.Contains(cell))
            {
                return true;
            }
        }

        return false;
    }

    public void PlaceHazards()
    {
        if (!roomAnalyzer)
        {
            return;
        }
        Vector2Int spawnCell;
        Vector2Int flipAxis;

        TrapData.Instance.GetTrapInfo(TrapType.ArrowTrap, out TrapData.TrapInfo trapInfo);
        List<Func<List<Vector2Int>, bool>> trapFilters = TrapData.Instance.GetFiltersForTrap(TrapType.ArrowTrap);

        switch (trapInfo.spawnRequirement)
        {
            case SpawnRequirement.SpawnPointOnly:
                if (roomAnalyzer.GetSpawnCell(trapFilters, out spawnCell, out flipAxis))
                {
                    GameObject spawnedTrap = Instantiate(trapInfo.trapPrefab, GetCellLocation(spawnCell) + new Vector3(0, 0.5f, 0), Quaternion.identity, roomContentsSimulateConditionally);
                    spawnedTrap.transform.localScale = new Vector3(flipAxis.x, flipAxis.y, spawnedTrap.transform.localScale.z);
                    Trap trap = spawnedTrap.GetComponent<Trap>();
                    if (trap)
                    {
                        trap.locatedInRoom = gridIndex;
                        trap.Init(trapInfo.damage);
                    }
                }
                break;
            case SpawnRequirement.PatrolPoints:
                break;
        }
        
        EnemyData.Instance.GetEnemyInfo(EnemyType.Patroller, out EnemyData.EnemyInfo enemyInfo);
        List<Func<List<Vector2Int>, bool>> enemyFilters = EnemyData.Instance.GetFiltersForEnemy(EnemyType.Patroller);
        
        switch (enemyInfo.spawnRequirement)
        {
            case SpawnRequirement.SpawnPointOnly:
                break;
            case SpawnRequirement.PatrolPoints:
                if (roomAnalyzer.GetPatrolPoints(enemyFilters, out (Vector2Int topLeft, Vector2Int topRight) patrolPoints))
                {
                    GameObject spawnedEnemy = Instantiate(enemyInfo.enemyPrefab, GetCellLocation(patrolPoints.topRight), Quaternion.identity, roomContentsSimulateConditionally);
                    Enemy enemy = spawnedEnemy.GetComponent<Enemy>();
                    if (enemy)
                    {
                        enemy.locatedInRoom = gridIndex;
                        enemy.Init(EnemyType.Patroller);
                    }
                }
                break;
        }
    }

    public void EvaluateSimulationState()
    {
        
    }
}
