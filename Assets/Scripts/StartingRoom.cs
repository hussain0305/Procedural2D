using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartingRoom : MonoBehaviour
{
    private RoomPlatformGenerator platformGenerator;
    private Room room;

    [HideInInspector] public Vector2Int playerStartPosition;
    [HideInInspector] public Vector2Int shopkeeperPosition;
    public void SetupStartingRoom(Room _room, RoomPlatformGenerator _platformGenerator)
    {
        room = _room;
        platformGenerator = _platformGenerator;

        AreaType[,] areaGrid = room.roomGridNPCAreas;
        
        int startX = (Global.CELL_SIZE_INTERIOR_X - Global.STARTING_AREA_WIDTH) / 2;
        int endX = (Global.CELL_SIZE_INTERIOR_X + Global.STARTING_AREA_WIDTH) / 2 - 1;
        int startY = (Global.CELL_SIZE_INTERIOR_Y - Global.STARTING_AREA_HEIGHT) / 2;
        int endY = (Global.CELL_SIZE_INTERIOR_Y + Global.STARTING_AREA_HEIGHT) / 2;
        bool openOnLeft = Random.Range(0, 10) < 5;
        int openSide = openOnLeft ? startX : endX;
        int closedSide = openOnLeft ? endX : startX;

        Vector2Int characterLeftPosition = new Vector2Int(startX + 2, startY + 2);
        Vector2Int characterRightPosition = new Vector2Int(endX - 2, startY + 2);
        playerStartPosition = openOnLeft ? characterRightPosition : characterLeftPosition;
        shopkeeperPosition = openOnLeft ? characterLeftPosition : characterRightPosition;
        
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                areaGrid[i, j] = AreaType.Shop;
                //construct ground and ceiling
                if (j == startY || j == endY)
                {
                    platformGenerator.PlacePlatformBlockAt(i, j, false);
                }
                //construct close wall
                if (i == closedSide)
                {
                    platformGenerator.PlacePlatformBlockAt(i, j, false);
                }
                //construct wall with exit
                if (i == openSide && (j <= startY + 1 || j >= endY - 1))
                {
                    platformGenerator.PlacePlatformBlockAt(i, j, false);
                }
            }
        }
    }
}
