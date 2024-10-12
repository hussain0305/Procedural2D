using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public Transform roomInterior;
    public Transform topLeftCorner;
    public Transform topRightCorner;
    public Transform bottomLeftCorner;
    public Transform bottomRightCorner;
    public RoomBorder leftWall;
    public RoomBorder rightWall;
    public RoomBorder ceiling;
    public RoomBorder floor;

    public SpriteRenderer roomSprite;

    public Tile wallTile;

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
    public bool[,] grid;

    [HideInInspector]
    public List<Vector2Int> pathwayCells = new List<Vector2Int>();

    private Tilemap wallTilemap;
    private Dictionary<Room, RoomOpening> neighbours = new Dictionary<Room, RoomOpening>();
    
    private void Awake()
    {
        SetScaleAndBorders();
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
            wallTilemap.SetTile(tilePosition, wallTile);
            tilePosition = new Vector3Int(x + roomWorldPosition.x, bottomLeftGridPosition.y + roomWorldPosition.y, 0);
            wallTilemap.SetTile(tilePosition, wallTile);
        }
        for (int y = bottomLeftGridPosition.y; y <= topLeftGridPosition.y; y++)
        {
            tilePosition = new Vector3Int(bottomLeftGridPosition.x + roomWorldPosition.x, y + roomWorldPosition.y, 0);
            wallTilemap.SetTile(tilePosition, wallTile);
            tilePosition = new Vector3Int(topRightGridPosition.x + roomWorldPosition.x, y + roomWorldPosition.y, 0);
            wallTilemap.SetTile(tilePosition, wallTile);
        }

        setupProgress.SetBorderTilesPlacementCompleted();
    }
    
    public void SetRoomProperties(RoomType _roomType, Vector2Int _gridIndex, Color _color, Tilemap _wallsTilemap)
    {
        roomType = _roomType;
        gridIndex = _gridIndex;
        roomSprite.color = _color;
        
        roomVolume = GetComponentInChildren<RoomInterior>();
        roomVolume.gridIndex = gridIndex;

        wallTilemap = _wallsTilemap;
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
                wallTilemap.SetTile(blockPosition, null);  
            }
        }
        setupProgress.SetPathTilesRemovalCompleted();
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
        roomAnalyzer?.AnalyzeRoom(grid);
    }
}
