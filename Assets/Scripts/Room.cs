using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform roomInterior;
    public Transform leftWall;
    public Transform rightWall;
    public Transform ceiling;
    public Transform floor;
    public Transform topLeftCorner;
    public Transform topRightCorner;
    public Transform bottomLeftCorner;
    public Transform bottomRightCorner;

    public SpriteRenderer roomSprite;
    public SpriteRenderer[] borderSprites;
    
    [HideInInspector]
    public Vector2Int gridIndex;

    [HideInInspector]
    public RoomType roomType;

    [HideInInspector]
    public int roomNumber;

    [HideInInspector]
    public RoomInterior roomVolume;

    private void Awake()
    {
        SetScaleAndBorders();
    }

    private void SetScaleAndBorders()
    {
        roomInterior.localScale = new Vector3(Global.CELL_SIZE_INTERIOR_X, Global.CELL_SIZE_INTERIOR_Y, 1);
        float edgeDistanceFromCenter_x = 0.5f * (Global.CELL_SIZE_INTERIOR_X + leftWall.localScale.x);
        float edgeDistanceFromCenter_y = 0.5f * (Global.CELL_SIZE_INTERIOR_Y + leftWall.localScale.x);

        Vector2Int leftDirection = Global.GetDirection(RoomEdge.Left);
        leftWall.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * leftDirection.x, edgeDistanceFromCenter_y * leftDirection.y);
        leftWall.localScale = new Vector3(1, Global.CELL_SIZE_INTERIOR_Y, 1);
        
        Vector2Int rightDirection = Global.GetDirection(RoomEdge.Right);
        rightWall.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * rightDirection.x, edgeDistanceFromCenter_y * rightDirection.y);
        rightWall.localScale = new Vector3(1, Global.CELL_SIZE_INTERIOR_Y, 1);

        Vector2Int ceilingDirection = Global.GetDirection(RoomEdge.Ceiling);
        ceiling.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * ceilingDirection.x, edgeDistanceFromCenter_y * ceilingDirection.y);
        ceiling.localScale = new Vector3(1, Global.CELL_SIZE_INTERIOR_X, 1);

        Vector2Int floorDirection = Global.GetDirection(RoomEdge.Floor);
        floor.transform.localPosition = new Vector3(edgeDistanceFromCenter_x * floorDirection.x, edgeDistanceFromCenter_y * floorDirection.y);
        floor.localScale = new Vector3(1, Global.CELL_SIZE_INTERIOR_X, 1);
        
        topLeftCorner.transform.localPosition = new Vector3(-1 * edgeDistanceFromCenter_x, edgeDistanceFromCenter_y);
        topRightCorner.transform.localPosition = new Vector3(edgeDistanceFromCenter_x, edgeDistanceFromCenter_y);
        bottomLeftCorner.transform.localPosition = new Vector3(-1 * edgeDistanceFromCenter_x, -1 * edgeDistanceFromCenter_y);
        bottomRightCorner.transform.localPosition = new Vector3(edgeDistanceFromCenter_x, -1 * edgeDistanceFromCenter_y);
    }
    
    public void SetRoomProperties(RoomType _roomType, Vector2Int _gridIndex, Color _color)
    {
        roomType = _roomType;
        gridIndex = _gridIndex;
        roomSprite.color = _color;

        foreach (SpriteRenderer spr in borderSprites)
        {
            spr.color = _color;
        }

        roomVolume = GetComponentInChildren<RoomInterior>();
        roomVolume.gridIndex = gridIndex;
    }

    public void CreateOpening(RoomEdge _edge, RoomBorderHorizontal _panel)
    {
        foreach (RoomBorderPanel panel in GetComponentsInChildren<RoomBorderPanel>())
        {
            if (panel.edge == _edge && panel.horizontalPanel == _panel)
            {
                Destroy(panel.gameObject);
                break;
            }
        }
    }
    public void CreateOpening(RoomEdge _edge, RoomBorderVertical _panel)
    {
        foreach (RoomBorderPanel panel in GetComponentsInChildren<RoomBorderPanel>())
        {
            if (panel.edge == _edge && panel.verticalPanel == _panel)
            {
                Destroy(panel.gameObject);
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
