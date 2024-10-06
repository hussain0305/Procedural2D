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
    
    [HideInInspector]
    public Vector2Int gridIndex;

    [HideInInspector]
    public RoomType roomType;

    [HideInInspector]
    public int roomNumber;

    [HideInInspector]
    public RoomInterior roomVolume;

    private Tilemap wallTilemap;

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
    }

    private void PlaceBorderTiles()
    {
        IEnumerator DelayedDrawTiles()
        {
            yield return null;
            
            Vector2Int roomWorldPosition = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
            int roomHalfWidth = (int)Global.CELL_SIZE_INTERIOR_X / 2;
            int roomHalfHeight = (int)Global.CELL_SIZE_INTERIOR_Y / 2;
            
            Vector2Int topLeftGridPosition = new Vector2Int(-roomHalfWidth - 1, roomHalfHeight);
            Vector2Int bottomRightGridPosition = new Vector2Int(roomHalfWidth, -roomHalfHeight - 1);
            Vector2Int topRightGridPosition = new Vector2Int(roomHalfWidth, roomHalfHeight);
            Vector2Int bottomLeftGridPosition = new Vector2Int(-roomHalfWidth - 1, -roomHalfHeight - 1);
            Vector3Int tilePosition = new Vector3Int(topLeftGridPosition.x + roomWorldPosition.x, topLeftGridPosition.y + roomWorldPosition.y, 0);
            
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
        }

        StartCoroutine(DelayedDrawTiles());
    }
    
    public void SetRoomProperties(RoomType _roomType, Vector2Int _gridIndex, Color _color, Tilemap _wallsTilemap)
    {
        roomType = _roomType;
        gridIndex = _gridIndex;
        roomSprite.color = _color;
        
        roomVolume = GetComponentInChildren<RoomInterior>();
        roomVolume.gridIndex = gridIndex;

        wallTilemap = _wallsTilemap;
        // PlaceBorderTiles();
    }

    public void CreateOpening(RoomEdge _edge, int[] path)
    {
        foreach (RoomBorder roomBorder in GetComponentsInChildren<RoomBorder>())
        {
            if (roomBorder.edge == _edge)
            {
                for (int i = path[0]; i <= path[path.Length - 1]; i++)
                {
                    if (roomBorder.borderBlock[i] != null)
                    {
                        Destroy(roomBorder.borderBlock[i]);
                        roomBorder.borderBlock[i] = null;
                    }
                }
                break;
            }
        }
    }

    public void SpawnPickupInSector(PickupType pickup)
    {
        GameObject pickupPrefab = GlobalData.Instance.GetPickupPrefab(pickup);
        if (pickupPrefab)
        {
            Instantiate(pickupPrefab, transform.position - new Vector3(4, 4, 0), Quaternion.identity, transform);
        }
    }
}
