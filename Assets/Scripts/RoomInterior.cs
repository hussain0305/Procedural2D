using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInterior : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int gridIndex;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.gameStarted && other && other.gameObject && other.gameObject.GetComponent<PlayerController>())
        {
            EventManager.OnRoomEntered?.Invoke(gridIndex);
        }
    }
}
