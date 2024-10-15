using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPlatformGenerator : MonoBehaviour
{
    public PlatformBlock platformPrefab;
    public Tile platformTile;

    public Room room;
    public int minPlatformHeight = 1;
    public int maxPlatformHeight = 3;
    public int minHorizontalGap = 1;
    public int minVerticalGap = 2;

    private int roomWidth;
    private int roomHeight;
    private bool[,] grid;
    private Vector2Int roomWorldPosition;
    private Tilemap platformTilemap;
    private float platformChance;
    private List<Vector2Int> pathwayCells;
    
    public void Init(Tilemap _platformTilemap, List<Vector2Int> roomPathways)
    {
        pathwayCells = roomPathways;
        platformChance = Random.Range(Global.PLATFORM_CHANCE - Global.PLATFORM_CHANCE_RANDOMIZATION, Global.PLATFORM_CHANCE + Global.PLATFORM_CHANCE_RANDOMIZATION);
        roomWidth = Global.CELL_SIZE_INTERIOR_X;
        roomHeight = Global.CELL_SIZE_INTERIOR_Y;
        grid = room.roomGridBuildings;
        roomWorldPosition = new Vector2Int((int)room.gameObject.transform.position.x, (int)room.gameObject.transform.position.y);

        platformTilemap = _platformTilemap;
    }

    public void GeneratePlatforms()
    {
        int halfRoomHeight = roomHeight / 2;
        int halfRoomWidth = roomWidth / 2;
        
        for (int y = -halfRoomHeight; y < halfRoomHeight; y++)
        {
            for (int x = -halfRoomWidth; x < halfRoomWidth / 2; x++)
            {
                int gridX = x + halfRoomWidth;
                int gridY = y + halfRoomHeight;

                if (Random.value < platformChance && !grid[gridX, gridY])
                {
                    int platformLength = Random.Range(2, 6);
                    int platformHeight = Random.Range(minPlatformHeight, maxPlatformHeight + 1);

                    if (CanPlacePlatform(gridX, gridY, platformLength, platformHeight))
                    {
                        PlacePlatform(x, y, platformLength, platformHeight);
                    }
                }
            }
        }

        room.setupProgress.SetPlatformsGenerationCompleted();
    }

    bool CanPlacePlatform(int gridX, int gridY, int length, int height)
    {
        if (gridX + length > roomWidth || gridX < 0)
        {
            return false;
        }

        if (gridY - height < 0 || gridY > roomHeight)
        {
            return false;
        }

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (room.roomGridNPCAreas[gridX + i, gridY - j] != AreaType.Free)
                {
                    return false;
                }
            }

        }

        for (int i = -minHorizontalGap; i < length + minHorizontalGap; i++)
        {
            if (gridX + i >= 0 && gridX + i < roomWidth)
            {
                for (int j = 0; j < height; j++)
                {
                    if (gridY - j >= 0 && grid[gridX + i, gridY - j])
                    {
                        return false;
                    }
                }
            }
        }

        for (int i = 0; i < length; i++)
        {
            for (int j = -minVerticalGap; j < height + minVerticalGap; j++)
            {
                if (gridY - j >= 0 && gridY - j < roomHeight && grid[gridX + i, gridY - j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    void PlacePlatform(int x, int y, int length, int height)
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int gridX = x + roomWidth / 2 + i;
                int gridY = y + roomHeight / 2 - j;
                Vector2Int gridPos = new Vector2Int(gridX, gridY);
                
                if (!pathwayCells.Contains(gridPos))
                {
                    PlacePlatformBlockAt(gridX, gridY, true);
                }
            }
        }

        room.setupProgress.SetPlatformTilesCompleted();
        room.roomGridBuildings = grid;
    }

    public void PlacePlatformBlockAt(int gridX, int gridY, bool isDamageable)
    {
        int halfRoomWidth = roomWidth / 2;
        int halfRoomHeight = roomHeight / 2;

        int gridXWithPositionalOffset = gridX - halfRoomWidth;
        int gridYWithPositionalOffset = gridY - halfRoomHeight;
        
        grid[gridX, gridY] = true;
        Vector3Int tilePosition = new Vector3Int(gridXWithPositionalOffset + roomWorldPosition.x, gridYWithPositionalOffset + roomWorldPosition.y, 0);
        platformTilemap.SetTile(tilePosition, platformTile);

        PlatformBlock block = Instantiate(platformPrefab, transform);
        block.isDestructible = isDamageable;
        block.tilePosition = tilePosition;
        block.platformTilemap = platformTilemap;
        // 0.5f needs to be added because the blocks are 1x1 and the anchor is at the center
        float localX = gridXWithPositionalOffset + 0.5f;
        float localY = gridYWithPositionalOffset + 0.5f;

        block.transform.localPosition = new Vector3(localX, localY, 0);
    }
}
