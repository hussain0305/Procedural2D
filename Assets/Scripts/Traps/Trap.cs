using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [HideInInspector] public Vector2Int locatedInRoom;
    [HideInInspector] public bool simulate;

    public virtual void Init(int _damage)
    {
        EventManager.OnRoomEntered += HandleRoomEntered;
    }

    private void HandleRoomEntered(Vector2Int gridIndex)
    {
        simulate = GameManager.Instance.ShouldSimulate(locatedInRoom, gridIndex);
    }
    
    private void OnDestroy()
    {
        EventManager.OnRoomEntered -= HandleRoomEntered;
    }
}
