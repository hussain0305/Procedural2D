using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PlatformBlock : MonoBehaviour
{
    [FormerlySerializedAs("gridIndex")] [HideInInspector]
    public Vector3Int tilePosition;

    [HideInInspector]
    public Tilemap platformTilemap;

    public void DestroyBlock()
    {
        platformTilemap.SetTile(tilePosition, null);
        Destroy(this.gameObject);
    }
}
