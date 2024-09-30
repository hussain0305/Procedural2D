using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public SpriteRenderer roomSprite;
    
    [HideInInspector]
    public Vector2Int gridIndex;

    [HideInInspector]
    public RoomType roomType;

    [HideInInspector]
    public int roomNumber;

    private void Awake()
    {
        SetScale();
    }

    private void SetScale()
    {
        transform.localScale = new Vector3(Global.CELL_SIZE, Global.CELL_SIZE, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.GetComponent<PlayerController>())
        {
            EventManager.OnRoomEntered?.Invoke(gridIndex);
        }
    }

    public void SetRoomProperties(RoomType _roomType, Vector2Int _gridIndex, Color _color)
    {
        roomType = _roomType;
        gridIndex = _gridIndex;
        roomSprite.color = _color;
    }
}
