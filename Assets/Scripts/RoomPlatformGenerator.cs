using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public Tile platformTile;

    public Room room;
    public int roomWidth;
    public int roomHeight;
    public float platformChance = 0.3f;
    public int minPlatformHeight = 1;
    public int maxPlatformHeight = 3;
    public int minHorizontalGap = 1;
    public int minVerticalGap = 2;

    private bool[,] grid;
    private Vector2Int roomWorldPosition;
    private Tilemap platformTilemap;

    public void Init(Tilemap _platformTilemap)
    {
        IEnumerator DelayedInit()
        {
            yield return null;
            roomWidth = Global.CELL_SIZE_INTERIOR_X;
            roomHeight = Global.CELL_SIZE_INTERIOR_Y;
            grid = new bool[roomWidth, roomHeight];
            roomWorldPosition = new Vector2Int((int)room.gameObject.transform.position.x, (int)room.gameObject.transform.position.y);

            platformTilemap = _platformTilemap;
            GeneratePlatforms();
        }

        StartCoroutine(DelayedInit());
    }

    void GeneratePlatforms()
    {
        for (int y = -(roomHeight / 2); y < roomHeight / 2; y++)
        {
            for (int x = -(roomWidth / 2); x < roomWidth / 2; x++)
            {
                int gridX = x + roomWidth / 2;
                int gridY = y + roomHeight / 2;

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
        // Mark the grid tiles as occupied
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int gridX = x + roomWidth / 2 + i;
                int gridY = y + roomHeight / 2 - j;
                grid[gridX, gridY] = true;

                Vector3Int tilePosition = new Vector3Int(x + i + roomWorldPosition.x, y - j + roomWorldPosition.y, 0);
                platformTilemap.SetTile(tilePosition, platformTile);
            }
        }

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject block = Instantiate(platformPrefab, transform);

                // 0.5f needs to be added because the blocks are 1x1 and the anchor is at the center
                float localX = (x + i) + 0.5f;
                float localY = (y - j) + 0.5f;

                block.transform.localPosition = new Vector3(localX, localY, 0);
                // block.transform.localScale = new Vector3(1.0f / roomWidth, 1.0f / roomHeight, 1);
            }
        }

        room.setupProgress.SetPlatformTilesCompleted();
        room.grid = grid;
    }
}
