using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {MainPath, Optional, StartingRoom, DestinationRoom, Wall, GridBorder}

public struct RoomProperties
{
    private Vector2Int roomPosition;
    private RoomType roomType;
    private Vector2Int enterFromRoom;
    private Vector2Int exitToRoom;
}

public struct MapProperties
{
    public int numRooms_MainPath;
    public Room startingRoom;
    public Room destinationRoom;
    public Dictionary<int, Room> mainPathRooms;
}

public enum Sector {
    LeftTop,
    CenterTop,
    RightTop,
    LeftMid,
    CenterMid,
    RightMid,
    LeftBottom,
    CenterBottom,
    RightBottom
}

public class Global : MonoBehaviour
{
    public const int GRID_SIZE = 6;
    public const int CELL_SIZE = 10;
    public const int MIN_ROOMS_IN_MAIN_PATH = 14;
    public const int MAX_ROOMS_IN_MAIN_PATH = 20;
}
