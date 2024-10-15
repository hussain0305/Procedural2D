using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PlatformBlock : Damageable
{
    [FormerlySerializedAs("gridIndex")] [HideInInspector]
    public Vector3Int tilePosition;

    [HideInInspector]
    public Tilemap platformTilemap;

    [HideInInspector]
    public bool isDestructible = true;

    public override void DealDamage(int damageAmount)
    {
        if (isDestructible)
        {
            platformTilemap.SetTile(tilePosition, null);
            Destroy(this.gameObject);
        }
    }
}
