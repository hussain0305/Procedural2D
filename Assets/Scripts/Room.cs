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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.GetComponent<PlayerController>())
        {
            EventManager.OnRoomEntered?.Invoke(gridIndex);
        }
    }
}
