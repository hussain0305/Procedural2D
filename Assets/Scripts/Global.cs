using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {MainPath, Optional, StartingRoom, DestinationRoom, Wall, GridBorder}

[System.Serializable]
public enum Ability { MultiJump, WallGrab}

[System.Serializable]
public enum PickupType { MultiJump, WallGrab}

[System.Serializable]
public enum PurchableItemType { MultiJump, WallGrab, Dash}

[System.Serializable]
public struct PurchasableItemInfo
{
    public PurchableItemType itemType;
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public int price;
    public int inventoryCount;
    public Action OnPurchaseAction; 
}

[System.Serializable]
public enum AreaType { Free, Shop}

[System.Serializable]
public enum PopupType { NPC, PurchasableItem, ItemInfo, InteractionPrompt }

[System.Serializable]
public struct PickupPrefabs
{
    public PickupType pickupType;
    public GameObject prefab;
}

[System.Serializable]
public struct RoomColors
{
    public RoomType roomtype;
    public Color color;
}

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

[System.Serializable]
public enum RoomEdge {
    Left,
    Right,
    Ceiling,
    Floor
}

public struct RoomPaths
{
    public Vector2Int room1;
    public Vector2Int room2;

    public RoomPaths(Vector2Int _room1, Vector2Int _room2)
    {
        room1 = _room1;
        room2 = _room2;
    }
}

public struct RoomOpening
{
    public RoomEdge edge;
    public int[] path;
    public List<Vector3Int> blockPositions;
    
    public RoomOpening(RoomEdge _edge, int[] _path, List<Vector3Int> _blockPositions)
    {
        edge = _edge;
        path = _path;
        blockPositions = _blockPositions;
    }
}

[System.Serializable]
public enum MovementType
{
    Walking,
    Flying
}

[System.Serializable]
public enum AttackType
{
    Melee,
    Ranged,
    Both
}

[System.Serializable]
public struct EnemyData
{
    public MovementType movementType;
    public AttackType attackType;
    public float patrolSpeed;
    public float pursueSpeed;
    public float attackRange;
    public Transform pointA;
    public Transform pointB;
}

public struct RoomSetupProgress
{
    private bool platformsGenerated;
    private bool platformTilesDone;
    private bool borderBlocksPlaced;
    private bool borderTilesDone;
    private bool pathsCreated;
    private bool pathTilesRemoved;

    public void SetPlatformsGenerationCompleted()
    {
        platformsGenerated = true;
    }
    
    public void SetPlatformTilesCompleted()
    {
        platformTilesDone = true;
    }
    
    public void SetBorderBlocksPlacementCompleted()
    {
        borderBlocksPlaced = true;
    }
    
    public void SetBorderTilesPlacementCompleted()
    {
        borderTilesDone = true;
    }
    
    public void SetPathCreationCompleted()
    {
        pathsCreated = true;
    }
    
    public void SetPathTilesRemovalCompleted()
    {
        pathTilesRemoved = true;
    }

    public bool GetBorderTilesPlaced()
    {
        return borderTilesDone;
    }

    public bool LayoutCompleted()
    {
        return platformsGenerated && platformTilesDone && borderBlocksPlaced && borderTilesDone && pathsCreated &&
               pathTilesRemoved;
    }
}

public struct Abilities
{
    public bool hasWallGrab;
    public bool hasDash;
    public int additionalJumps;
}

[System.Serializable]
public enum BulletType
{
    TrapArrow,
    Laser
}

[System.Serializable]
public enum TrapType
{
    ArrowTrap,
}

public class Global : MonoBehaviour
{
    public static Dictionary<RoomEdge, Vector2Int> Directions
    {
        get
        {
            if (directions == null)
            {
                directions = new Dictionary<RoomEdge, Vector2Int>();
                directions.Add(RoomEdge.Ceiling, new Vector2Int(0, 1));
                directions.Add(RoomEdge.Right, new Vector2Int(1, 0));
                directions.Add(RoomEdge.Floor, new Vector2Int(0, -1));
                directions.Add(RoomEdge.Left, new Vector2Int(-1, 0));
            }

            return directions;
        }
    }
    
    private static Dictionary<RoomEdge, Vector2Int> directions;
    
    public const int GRID_SIZE = 8;
    public const int CELL_SIZE_INTERIOR_X = 22;
    public const int CELL_SIZE_INTERIOR_Y = 12;
    public const int CELL_WALL_SIZE = 2;
    public const int MIN_PATH_WIDTH = 2;
    public const int MAX_PATH_WIDTH = 5;
    public const int PATHWAY_WIDTH = 2;
    public const int MIN_ROOMS_IN_MAIN_PATH = 14;
    public const int MAX_ROOMS_IN_MAIN_PATH = 20;
    
    //Platform Generation
    public const float PLATFORM_CHANCE = 0.2f;
    public const float PLATFORM_CHANCE_RANDOMIZATION = 0.075f;
    public const int PLATFORM_MIN_HEIGHT = 1;
    public const int PLATFORM_MAX_HEIGHT = 3;
    public const int PLATFORM_MIN_LENGTH = 2;
    public const int PLATFORM_MAX_LENGTH = 9;
    public const int PLATFORM_MIN_HORIZONTAL_GAP = 1;
    public const int PLATFORM_MIN_VERTICAL_GAP = 2;

    
    //Room Analysis
    public const int VERTICAL_LANE_THRESHOLD = 6;
    public const int HORIZONTAL_LANE_THRESHOLD = 10;
    
    //Starting Room
    public const int STARTING_AREA_HEIGHT = 5;
    public const int STARTING_AREA_WIDTH = 14;
    
    //UI Prompts
    public const float POPUP_VERTICAL_OFFSET = 1;
    public const float POPUP_HORIZONTAL_OFFSET = 0;

    public static Vector2Int GetDirection(RoomEdge edge)
    {
        return Directions[edge];
    }

    public static Vector3 GetPromptOffset()
    {
        return new Vector3(POPUP_HORIZONTAL_OFFSET, POPUP_VERTICAL_OFFSET, 0);
    }
    
    public static Vector3 GetPromptOffset(Vector3 additionalOffset)
    {
        return new Vector3(POPUP_HORIZONTAL_OFFSET + additionalOffset.x, POPUP_VERTICAL_OFFSET + additionalOffset.y, 0);
    }

}
