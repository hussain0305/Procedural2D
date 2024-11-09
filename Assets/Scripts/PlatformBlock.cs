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

    public override void TakeDamage(int damageAmount)
    {
        if (isDestructible)
        {
            PFXManager.Instance.PlayPFX(PFXDatabase.PFXType.PlatformBlockBreak, transform.position);
            platformTilemap.SetTile(tilePosition, null);
            Destroy(this.gameObject);
        }
    }
}
