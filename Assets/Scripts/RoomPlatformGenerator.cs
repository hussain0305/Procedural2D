using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public int roomWidth;
    public int roomHeight;
    public float platformChance = 0.3f;
    public int minPlatformHeight = 1;
    public int maxPlatformHeight = 3;
    public int minHorizontalGap = 1;
    public int minVerticalGap = 2;

    private bool[,] grid;

    void Start()
    {
        roomWidth = Global.CELL_SIZE_INTERIOR_X;
        roomHeight = Global.CELL_SIZE_INTERIOR_Y;
        grid = new bool[roomWidth, roomHeight];
        GeneratePlatforms();
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
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int gridX = x + roomWidth / 2 + i;
                int gridY = y + roomHeight / 2 - j;
                grid[gridX, gridY] = true;
            }
        }

        GameObject platform = Instantiate(platformPrefab, transform);

        float localX = x + (length / 2.0f);
        float localY = y - (height / 2.0f);
        platform.transform.localPosition = new Vector3(localX / roomWidth, localY / roomHeight, 0);
        platform.transform.localScale = new Vector3((float)length / roomWidth, (float)height / roomHeight, 1);
    }
}
