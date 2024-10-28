using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [HideInInspector] public Vector2Int locatedInRoom;

    public virtual void Init(int _damage)
    {
        
    }
}
