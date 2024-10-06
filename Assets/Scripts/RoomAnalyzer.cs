using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomAnalyzer : MonoBehaviour
{
    // Gizmo visibility toggles
    public static bool showHorizontalLanes = true;
    public static bool showVerticalLanes = true;
    public static bool showSpaciousAreas = true;
    public static bool showClosedAreas = true;
    
    private bool[,] grid; // false = empty, true = occupied

    private int roomWidth;
    private int roomHeight;

    private List<Vector2Int> horizontalLanes = new List<Vector2Int>();
    private List<Vector2Int> verticalLanes = new List<Vector2Int>();
    private List<List<Vector2Int>> spaciousAreas = new List<List<Vector2Int>>();
    private List<List<Vector2Int>> closedAreas = new List<List<Vector2Int>>();
    
    public void AnalyzeRoom(bool[,] _grid)
    {
        grid = _grid;
        roomWidth = Global.CELL_SIZE_INTERIOR_X;
        roomHeight = Global.CELL_SIZE_INTERIOR_Y;
        
        // Analyze lanes
        horizontalLanes = DetectHorizontalLanes();
        verticalLanes = DetectVerticalLanes();
        
        // Analyze spacious areas
        spaciousAreas = DetectSpaciousAreas();

        // Analyze closed areas
        closedAreas = DetectClosedAreas();

        // Output the results
        Debug.Log($"Detected {horizontalLanes.Count} horizontal lanes.");
        Debug.Log($"Detected {verticalLanes.Count} vertical lanes.");
        Debug.Log($"Detected {spaciousAreas.Count} spacious areas.");
        Debug.Log($"Detected {closedAreas.Count} closed areas.");
    }

    // Method to detect horizontal lanes
    List<Vector2Int> DetectHorizontalLanes()
    {
        List<Vector2Int> horizontalLanes = new List<Vector2Int>();

        for (int y = 0; y < roomHeight; y++)
        {
            int laneLength = 0;
            for (int x = 0; x < roomWidth; x++)
            {
                if (!grid[x, y]) // Empty cell
                {
                    laneLength++;
                }
                else // Encounter a platform or wall
                {
                    if (laneLength >= 4) // Check if we have a valid lane
                    {
                        for (int i = x - laneLength; i < x; i++)
                        {
                            horizontalLanes.Add(new Vector2Int(i, y));
                        }
                    }
                    laneLength = 0; // Reset the lane length
                }
            }
            // Handle the last row part if it ends with an empty space
            if (laneLength >= 4)
            {
                for (int i = roomWidth - laneLength; i < roomWidth; i++)
                {
                    horizontalLanes.Add(new Vector2Int(i, y));
                }
            }
        }

        return horizontalLanes;
    }

    // Method to detect vertical lanes
    List<Vector2Int> DetectVerticalLanes()
    {
        List<Vector2Int> verticalLanes = new List<Vector2Int>();

        for (int x = 0; x < roomWidth; x++)
        {
            int laneLength = 0;
            for (int y = 0; y < roomHeight; y++)
            {
                if (!grid[x, y]) // Empty cell
                {
                    laneLength++;
                }
                else // Encounter a platform or wall
                {
                    if (laneLength >= 4) // Check if we have a valid lane
                    {
                        for (int i = y - laneLength; i < y; i++)
                        {
                            verticalLanes.Add(new Vector2Int(x, i));
                        }
                    }
                    laneLength = 0; // Reset the lane length
                }
            }
            // Handle the last column part if it ends with an empty space
            if (laneLength >= 4)
            {
                for (int i = roomHeight - laneLength; i < roomHeight; i++)
                {
                    verticalLanes.Add(new Vector2Int(x, i));
                }
            }
        }

        return verticalLanes;
    }

    // Method to detect spacious areas (75% or more free space)
    List<List<Vector2Int>> DetectClosedAreas()
    {
        List<List<Vector2Int>> closedAreas = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (!grid[x, y] && !visited[x, y])
                {
                    List<Vector2Int> region = new List<Vector2Int>();
                    FloodFill(x, y, region, visited);

                    int entranceCount = 0;

                    foreach (Vector2Int cell in region)
                    {
                        // Check if this region is connected to the outer boundaries or walls
                        if (IsOnEdge(cell.x, cell.y) || IsAdjacentToWall(cell.x, cell.y))
                        {
                            entranceCount++;
                        }
                    }

                    // If there are only 1 or 2 entrances, classify this region as closed
                    if (entranceCount <= 2)
                    {
                        closedAreas.Add(region);
                    }
                }
            }
        }

        return closedAreas;
    }

    // Updated Spacious Areas Detection
    List<List<Vector2Int>> DetectSpaciousAreas()
    {
        List<List<Vector2Int>> spaciousAreas = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (!grid[x, y] && !visited[x, y])
                {
                    List<Vector2Int> region = new List<Vector2Int>();
                    FloodFill(x, y, region, visited);

                    int totalCells = region.Count;
                    int freeCells = 0;

                    // Count the number of free cells
                    foreach (Vector2Int cell in region)
                    {
                        if (!grid[cell.x, cell.y])
                        {
                            freeCells++;
                        }
                    }

                    // Define criteria for spacious areas:
                    // - Region must be at least 8 cells in size
                    // - At least 70% of the region must be free space
                    if (totalCells >= 8 && ((float)freeCells / totalCells) >= 0.7f)
                    {
                        spaciousAreas.Add(region);
                    }
                }
            }
        }

        return spaciousAreas;
    }
    
    // Flood fill algorithm to explore connected regions
    void FloodFill(int x, int y, List<Vector2Int> region, bool[,] visited)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(x, y));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            if (current.x >= 0 && current.x < roomWidth && current.y >= 0 && current.y < roomHeight && !visited[current.x, current.y] && !grid[current.x, current.y])
            {
                visited[current.x, current.y] = true;
                region.Add(current);

                // Check adjacent cells
                stack.Push(new Vector2Int(current.x + 1, current.y));
                stack.Push(new Vector2Int(current.x - 1, current.y));
                stack.Push(new Vector2Int(current.x, current.y + 1));
                stack.Push(new Vector2Int(current.x, current.y - 1));
            }
        }
    }
    
    // Check if a cell is on the edge of the room
    bool IsEdgeCell(int x, int y)
    {
        return x == 0 || x == roomWidth - 1 || y == 0 || y == roomHeight - 1;
    }

    // Check if a cell has an adjacent open space
    bool IsAdjacentToOpenSpace(int x, int y)
    {
        if (x > 0 && !grid[x - 1, y]) return true;
        if (x < roomWidth - 1 && !grid[x + 1, y]) return true;
        if (y > 0 && !grid[x, y - 1]) return true;
        if (y < roomHeight - 1 && !grid[x, y + 1]) return true;

        return false;
    }
    
    // Helper function to check if a cell is adjacent to a wall or boundary
    bool IsAdjacentToWall(int x, int y)
    {
        if (x > 0 && grid[x - 1, y]) return true;
        if (x < roomWidth - 1 && grid[x + 1, y]) return true;
        if (y > 0 && grid[x, y - 1]) return true;
        if (y < roomHeight - 1 && grid[x, y + 1]) return true;

        return false;
    }

    // Helper function to check if a cell is on the edge of the room
    bool IsOnEdge(int x, int y)
    {
        return x == 0 || x == roomWidth - 1 || y == 0 || y == roomHeight - 1;
    }
    
// Optional: Add a menu to toggle global Gizmos visibility
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

    // Draw Gizmos based on global visibility settings
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void DrawGizmos(RoomAnalyzer room, GizmoType gizmoType)
    {
        Vector3 roomBasePosition = room.transform.position;
        Vector3 offset = new Vector3(
            -1 + (float)(Global.CELL_SIZE_INTERIOR_X + 1) / 2, 
            -1 + (float)(Global.CELL_SIZE_INTERIOR_Y + 1) / 2, 0);

        // Draw Horizontal Lanes
        if (showHorizontalLanes)
        {
            Gizmos.color = Color.red;
            foreach (Vector2Int lane in room.horizontalLanes)
            {
                Vector3 pos = roomBasePosition + new Vector3(lane.x, lane.y, 0) - offset;
                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
            }
        }

        // Draw Vertical Lanes
        if (showVerticalLanes)
        {
            Gizmos.color = Color.white;
            foreach (Vector2Int lane in room.verticalLanes)
            {
                Vector3 pos = roomBasePosition + new Vector3(lane.x, lane.y, 0) - offset;
                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
            }
        }

        // Draw Spacious Areas
        if (showSpaciousAreas)
        {
            Gizmos.color = Color.magenta; // Semi-transparent green
            foreach (List<Vector2Int> area in room.spaciousAreas)
            {
                foreach (Vector2Int cell in area)
                {
                    Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }

        // Draw Closed Areas
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
    }
}

