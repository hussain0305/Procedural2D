using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomAnalyzer : MonoBehaviour
{
    // Gizmo visibility toggles
    public static bool showHorizontalLanes = false;
    public static bool showVerticalLanes = false;
    public static bool showSpaciousAreas = false;
    public static bool showClosedAreas = false;
    public static bool showFreeAreas = false;
    public static bool showOccupiedAreas = false;
    public static bool showPathways = false;
    
    private bool[,] grid; // false = empty, true = occupied

    private int roomWidth;
    private int roomHeight;

    [HideInInspector]
    public List<List<Vector2Int>> horizontalLanes = new List<List<Vector2Int>>();
    [HideInInspector]
    public List<List<Vector2Int>> verticalLanes = new List<List<Vector2Int>>();
    [HideInInspector]
    public List<List<Vector2Int>> spaciousAreas = new List<List<Vector2Int>>();
    [HideInInspector]
    public List<List<Vector2Int>> closedAreas = new List<List<Vector2Int>>();
    
    public void AnalyzeRoom(bool[,] _grid)
    {
        grid = _grid;
        roomWidth = Global.CELL_SIZE_INTERIOR_X;
        roomHeight = Global.CELL_SIZE_INTERIOR_Y;
        
        horizontalLanes = DetectHorizontalLanes();
        verticalLanes = DetectVerticalLanes();
        spaciousAreas = DetectSpaciousAreas();
        closedAreas = DetectClosedSpaces();
    }

    List<List<Vector2Int>> DetectHorizontalLanes()
    {
        horizontalLanes = new List<List<Vector2Int>>();

        for (int y = 0; y < roomHeight; y++)
        {
            int laneLength = 0;
            List<Vector2Int> currentLane = new List<Vector2Int>();

            for (int x = 0; x < roomWidth; x++)
            {
                if (!grid[x, y])
                {
                    laneLength++;
                    currentLane.Add(new Vector2Int(x, y));
                }
                else
                {
                    if (laneLength >= Global.HORIZONTAL_LANE_THRESHOLD)
                    {
                        horizontalLanes.Add(new List<Vector2Int>(currentLane));
                    }

                    laneLength = 0;
                    currentLane.Clear();
                }
            }

            if (laneLength >= Global.HORIZONTAL_LANE_THRESHOLD)
            {
                horizontalLanes.Add(new List<Vector2Int>(currentLane));
            }
        }

        return horizontalLanes;
    }

    List<List<Vector2Int>> DetectVerticalLanes()
    {
        verticalLanes = new List<List<Vector2Int>>();

        for (int x = 0; x < roomWidth; x++)
        {
            int laneLength = 0;
            List<Vector2Int> currentLane = new List<Vector2Int>();

            for (int y = 0; y < roomHeight; y++)
            {
                if (!grid[x, y])
                {
                    laneLength++;
                    currentLane.Add(new Vector2Int(x, y));
                }
                else
                {
                    if (laneLength >= Global.VERTICAL_LANE_THRESHOLD)
                    {
                        verticalLanes.Add(new List<Vector2Int>(currentLane));
                    }

                    laneLength = 0;
                    currentLane.Clear();
                }
            }

            if (laneLength >= Global.VERTICAL_LANE_THRESHOLD)
            {
                verticalLanes.Add(new List<Vector2Int>(currentLane));
            }
        }

        return verticalLanes;
    }

    List<List<Vector2Int>> DetectClosedSpaces()
    {
        List<List<Vector2Int>> closedSpaces = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (grid[x, y] || visited[x, y])
                    continue;

                List<Vector2Int> closedSpace = FindClosedSpace(x, y, visited);

                if (closedSpace.Count >= 4 && closedSpace.Count <= 9 && HasHindrancesOnThreeSides(closedSpace))
                {
                    closedSpaces.Add(closedSpace);
                    foreach (var cell in closedSpace)
                    {
                        visited[cell.x, cell.y] = true;
                    }
                }
            }
        }

        return closedSpaces;
    }

    List<Vector2Int> FindClosedSpace(int startX, int startY, bool[,] visited)
    {
        List<Vector2Int> area = new List<Vector2Int>();

        for (int width = 2; width <= 3; width++)
        {
            for (int height = 2; height <= 3; height++)
            {
                if (startX + width > roomWidth || startY + height > roomHeight)
                    continue;

                if (IsClosedArea(startX, startY, width, height, visited))
                {
                    for (int x = startX; x < startX + width; x++)
                    {
                        for (int y = startY; y < startY + height; y++)
                        {
                            area.Add(new Vector2Int(x, y));
                        }
                    }

                    return area;
                }
            }
        }

        MarkVisited(startX, startY, visited, 3, 3);

        return area;
    }

    bool IsClosedArea(int startX, int startY, int width, int height, bool[,] visited)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (visited[x, y] || grid[x, y])
                    return false;
            }
        }

        for (int x = startX - 1; x <= startX + width; x++)
        {
            for (int y = startY - 1; y <= startY + height; y++)
            {
                if (x >= 0 && x < roomWidth && y >= 0 && y < roomHeight && !grid[x, y] && !visited[x, y])
                    return false;
            }
        }

        return true;
    }

    void MarkVisited(int startX, int startY, bool[,] visited, int width, int height)
    {
        for (int x = startX; x < startX + width && x < roomWidth; x++)
        {
            for (int y = startY; y < startY + height && y < roomHeight; y++)
            {
                visited[x, y] = true;
            }
        }
    }

    bool HasHindrancesOnThreeSides(List<Vector2Int> area)
    {
        HashSet<string> hindrances = new HashSet<string>();

        foreach (var cell in area)
        {
            if (cell.x == 0 || grid[cell.x - 1, cell.y]) hindrances.Add("left");
            if (cell.x == roomWidth - 1 || grid[cell.x + 1, cell.y]) hindrances.Add("right");
            if (cell.y == 0 || grid[cell.x, cell.y - 1]) hindrances.Add("bottom");
            if (cell.y == roomHeight - 1 || grid[cell.x, cell.y + 1]) hindrances.Add("top");
        }

        return hindrances.Count >= 3;
    }

    bool RaycastForOpenPath(Vector2Int cell, Vector2 direction)
    {
        Vector3 worldPosition = new Vector3(cell.x, cell.y, 0);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, direction, 1.0f); // 1 unit ray
        return hit.collider != null;
    }

    List<List<Vector2Int>> DetectSpaciousAreas()
    {
        spaciousAreas = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (grid[x, y] || visited[x, y])
                    continue;

                List<Vector2Int> spaciousArea = ExpandSpaciousArea(x, y);

                if (spaciousArea.Count >= 9)
                {
                    spaciousAreas.Add(spaciousArea);
                    foreach (var cell in spaciousArea)
                    {
                        visited[cell.x, cell.y] = true;
                    }
                }
            }
        }

        return spaciousAreas;
    }

    List<Vector2Int> ExpandSpaciousArea(int startX, int startY)
    {
        List<Vector2Int> area = new List<Vector2Int>();

        if (!Is3x3AreaFree(startX, startY))
            return area;

        for (int x = startX; x < startX + 3; x++)
        {
            for (int y = startY; y < startY + 3; y++)
            {
                area.Add(new Vector2Int(x, y));
            }
        }

        int width = 3;
        int height = 3;
        bool freezeX = false;
        bool freezeY = false;

        while (!freezeX || !freezeY)
        {
            if (!freezeX && CanExpandX(startX, startY, width, height))
            {
                for (int y = startY; y < startY + height; y++)
                {
                    area.Add(new Vector2Int(startX + width, y));
                }
                width++;
            }
            else
            {
                freezeX = true;
            }

            if (!freezeY && CanExpandY(startX, startY, width, height))
            {
                for (int x = startX; x < startX + width; x++)
                {
                    area.Add(new Vector2Int(x, startY + height));
                }
                height++;
            }
            else
            {
                freezeY = true;
            }

            if (freezeX && freezeY)
            {
                break;
            }
        }

        return area;
    }

    bool Is3x3AreaFree(int startX, int startY)
    {
        if (startX + 2 >= roomWidth || startY + 2 >= roomHeight)
            return false;

        for (int x = startX; x < startX + 3; x++)
        {
            for (int y = startY; y < startY + 3; y++)
            {
                if (grid[x, y])
                    return false;
            }
        }
        return true;
    }

    bool CanExpandX(int startX, int startY, int width, int height)
    {
        if (startX + width >= roomWidth)
            return false;

        for (int y = startY; y < startY + height; y++)
        {
            if (grid[startX + width, y])
                return false;
        }

        return true;
    }

    bool CanExpandY(int startX, int startY, int width, int height)
    {
        if (startY + height >= roomHeight)
            return false;

        for (int x = startX; x < startX + width; x++)
        {
            if (grid[x, startY + height])
                return false;
        }

        return true;
    }
    
    public List<List<Vector2Int>> GetZones(Func<List<Vector2Int>, bool> filterPredicate)
    {
        List<List<Vector2Int>> matchingZones = new List<List<Vector2Int>>();

        foreach (List<Vector2Int> horizontalLane in horizontalLanes)
        {
            if (filterPredicate(horizontalLane))
            {
                matchingZones.Add(horizontalLane);
            }
        }

        foreach (List<Vector2Int> verticalLane in verticalLanes)
        {
            if (filterPredicate(verticalLane))
            {
                matchingZones.Add(verticalLane);
            }
        }

        foreach (List<Vector2Int> spaciousArea in spaciousAreas)
        {
            if (filterPredicate(spaciousArea))
            {
                matchingZones.Add(spaciousArea);
            }
        }

        return matchingZones;
    }
    
#if UNITY_EDITOR
    #region Gizmos
    private int currentSpaciousAreaIndex = 0;
    private float nextAreaTime = 0f;
    private float displayDuration = 2f;
    
    [UnityEditor.MenuItem("Gizmos/Show Horizontal Lanes(Red)")]
    public static void ToggleHorizontalLanes()
    {
        showHorizontalLanes = !showHorizontalLanes;
    }

    [UnityEditor.MenuItem("Gizmos/Show Vertical Lanes(White)")]
    public static void ToggleVerticalLanes()
    {
        showVerticalLanes = !showVerticalLanes;
    }

    [UnityEditor.MenuItem("Gizmos/Show Spacious Areas(Magenta)")]
    public static void ToggleSpaciousAreas()
    {
        showSpaciousAreas = !showSpaciousAreas;
    }

    [UnityEditor.MenuItem("Gizmos/Show Closed Areas(Yellow)")]
    public static void ToggleClosedAreas()
    {
        showClosedAreas = !showClosedAreas;
    }

    [UnityEditor.MenuItem("Gizmos/Show Free Areas(Green)")]
    public static void ToggleFreeAreas()
    {
        showFreeAreas = !showFreeAreas;
    }
    
    [UnityEditor.MenuItem("Gizmos/Show Occupied Areas(Cyan)")]
    public static void ToggleOccupiedAreas()
    {
        showOccupiedAreas = !showOccupiedAreas;
    }

    [UnityEditor.MenuItem("Gizmos/Show Pathways(Gray)")]
    public static void TogglePathways()
    {
        showPathways = !showPathways;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void DrawGizmos(RoomAnalyzer room, GizmoType gizmoType)
    {
        Vector3 roomBasePosition = room.transform.position;
        Vector3 offset = new Vector3(
            -1 + (float)(Global.CELL_SIZE_INTERIOR_X + 1) / 2, 
            -1 + (float)(Global.CELL_SIZE_INTERIOR_Y + 1) / 2, 0);

        if (showHorizontalLanes)
        {
            Gizmos.color = Color.red;
            foreach (List<Vector2Int> lane in room.horizontalLanes)
            {
                foreach (Vector2Int cell in lane)
                {
                    Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }

        if (showVerticalLanes)
        {
            Gizmos.color = Color.white;
            foreach (List<Vector2Int> lane in room.verticalLanes)
            {
                foreach (Vector2Int cell in lane)
                {
                    Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }

        if (showSpaciousAreas && room.spaciousAreas.Count > 0)
        {
            Gizmos.color = Color.magenta;

            List<Vector2Int> currentArea = room.spaciousAreas[room.currentSpaciousAreaIndex];
            foreach (Vector2Int cell in currentArea)
            {
                Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
            }
        }
        
        if (showClosedAreas)
        {
            Gizmos.color = Color.yellow;
            foreach (List<Vector2Int> area in room.closedAreas)
            {
                foreach (Vector2Int cell in area)
                {
                    Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }
        
        if (showFreeAreas)
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < room.roomWidth; x++)
            {
                for (int y = 0; y < room.roomHeight; y++)
                {
                    if (!room.grid[x, y])
                    {
                        Vector3 pos = roomBasePosition + new Vector3(x, y, 0) - offset;
                        Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                    }
                }
            }
        }
        
        if (showOccupiedAreas)
        {
            Gizmos.color = Color.cyan;
            for (int x = 0; x < room.roomWidth; x++)
            {
                for (int y = 0; y < room.roomHeight; y++)
                {
                    if (room.grid[x, y])
                    {
                        Vector3 pos = roomBasePosition + new Vector3(x, y, 0) - offset;
                        Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                    }
                }
            }
        }

        if (showPathways)
        {
            Gizmos.color = Color.gray;
            Room roomScript = room.GetComponentInParent<Room>();
            foreach (Vector2Int pathway in roomScript.pathwayCells)
            {
                Vector3 pos = roomBasePosition + new Vector3(pathway.x, pathway.y, 0) - offset;
                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
            }
        }
    }
    #endregion
    void Update()
    {
        if (Time.time > nextAreaTime && spaciousAreas != null && spaciousAreas.Count > 0)
        {
            currentSpaciousAreaIndex = (currentSpaciousAreaIndex + 1) % spaciousAreas.Count;
            nextAreaTime = Time.time + displayDuration;
        }
    }
#endif

}
