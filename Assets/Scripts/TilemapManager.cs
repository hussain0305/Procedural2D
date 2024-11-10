using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("Tiles")]
    public Tile wallTile;
    public Tile platformTile;

    [Header("Tilemaps")]
    public Tilemap wallOutlineTilemap;
    public Tilemap wallTilemap;
    public Tilemap platformOutlineTilemap;
    public Tilemap platformTilemap;

    public static TilemapManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaceWallTileAt(Vector3Int position)
    {
        wallTilemap.SetTile(position, wallTile);
        wallOutlineTilemap.SetTile(position, wallTile);
    }

    public void RemoveWallTileFrom(Vector3Int position)
    {
        wallTilemap.SetTile(position, null);
        wallOutlineTilemap.SetTile(position, null);
    }
    
    public void PlacePlatformTileAt(Vector3Int position)
    {
        platformTilemap.SetTile(position, platformTile);
        platformOutlineTilemap.SetTile(position, platformTile);
    }

    public void RemovePlatformileFrom(Vector3Int position)
    {
        platformTilemap.SetTile(position, null);
        platformOutlineTilemap.SetTile(position, null);
    }

    public void ClearPlatformTiles()
    {
        platformTilemap.ClearAllTiles();
        platformOutlineTilemap.ClearAllTiles();
    }
}
