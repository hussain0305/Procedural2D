using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomAnalyzer : MonoBehaviour
{
    // Gizmo visibility toggles
    public static bool showHorizontalLanes = false;
    public static bool showVerticalLanes = false;
    public static bool showSpaciousAreas = false;
    public static bool showClosedAreas = false;
    public static bool showFreeAreas = false;
    public static bool showOccupiedAreas = false;
    
    private bool[,] grid; // false = empty, true = occupied

    private int roomWidth;
    private int roomHeight;

    private List<List<Vector2Int>> horizontalLanes = new List<List<Vector2Int>>();
    private List<List<Vector2Int>> verticalLanes = new List<List<Vector2Int>>();
    private List<List<Vector2Int>> spaciousAreas = new List<List<Vector2Int>>();
    private List<List<Vector2Int>> closedAreas = new List<List<Vector2Int>>();
    
    public void AnalyzeRoom(bool[,] _grid)
    {
        grid = _grid;
        roomWidth = Global.CELL_SIZE_INTERIOR_X;
        roomHeight = Global.CELL_SIZE_INTERIOR_Y;
        
        horizontalLanes = DetectHorizontalLanes();
        verticalLanes = DetectVerticalLanes();
        spaciousAreas = DetectSpaciousAreas();
        closedAreas = DetectClosedAreas();
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

            // Handle the last row part if it ends with an empty space
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

    List<List<Vector2Int>> DetectClosedAreas()
    {
        List<List<Vector2Int>> closedAreas = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        // Loop over the grid and check for minimum 3x3 areas and larger
        for (int x = 0; x < roomWidth - 2; x++)
        {
            for (int y = 0; y < roomHeight - 2; y++)
            {
                // If this cell isn't visited and isn't a platform/wall
                if (!visited[x, y] && !grid[x, y])
                {
                    // Try expanding the area to find the largest valid closed area
                    List<Vector2Int> largestClosedArea = FindLargestClosedArea(x, y, visited);
                    if (largestClosedArea != null && largestClosedArea.Count >= 9)  // Must be at least 3x3
                    {
                        closedAreas.Add(largestClosedArea);
                    }
                }
            }
        }

        return closedAreas;
    }

    List<Vector2Int> FindLargestClosedArea(int startX, int startY, bool[,] visited)
    {
        List<Vector2Int> bestArea = null;
        int maxWidth = roomWidth - startX;
        int maxHeight = roomHeight - startY;

        // Try different widths and heights (starting from 3x3)
        for (int width = 3; width <= maxWidth; width++)
        {
            for (int height = 3; height <= maxHeight; height++)
            {
                List<Vector2Int> area = GetAreaCells(startX, startY, width, height);

                // Check if this area satisfies the closed area condition
                if (area.Count >= 9)  // Must be at least 3x3
                {
                    int closedSides = CountClosedSides(area);
                    bool thirdSidePartiallyBlocked = CheckThirdSidePartialBlock(area, width, height);

                    if (closedSides >= 2 && thirdSidePartiallyBlocked)
                    {
                        // This area is valid, so it becomes the best area found so far
                        bestArea = new List<Vector2Int>(area);
                        MarkCellsAsVisited(area, visited);
                    }
                    else
                    {
                        // If a larger area doesn't satisfy the condition, stop checking further heights for this width
                        break;
                    }
                }
            }
        }

        return bestArea;
    }

    List<Vector2Int> GetAreaCells(int startX, int startY, int width, int height)
    {
        List<Vector2Int> areaCells = new List<Vector2Int>();

        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (!grid[x, y])
                {
                    areaCells.Add(new Vector2Int(x, y));
                }
            }
        }
        return areaCells;
    }

    // Count the number of fully blocked sides for the area
    int CountClosedSides(List<Vector2Int> area)
    {
        int closedSides = 0;
        // Check if the area has at least 2 fully blocked sides
        if (IsSideFullyBlocked(area, "left")) closedSides++;
        if (IsSideFullyBlocked(area, "right")) closedSides++;
        if (IsSideFullyBlocked(area, "top")) closedSides++;
        if (IsSideFullyBlocked(area, "bottom")) closedSides++;

        return closedSides;
    }

    // Check if a specific side of the area is fully blocked
    bool IsSideFullyBlocked(List<Vector2Int> area, string side)
    {
        foreach (Vector2Int cell in area)
        {
            switch (side)
            {
                case "left":
                    if (cell.x == 0 || grid[cell.x - 1, cell.y]) continue;
                    else return false;
                case "right":
                    if (cell.x == roomWidth - 1 || grid[cell.x + 1, cell.y]) continue;
                    else return false;
                case "top":
                    if (cell.y == roomHeight - 1 || grid[cell.x, cell.y + 1]) continue;
                    else return false;
                case "bottom":
                    if (cell.y == 0 || grid[cell.x, cell.y - 1]) continue;
                    else return false;
            }
        }
        return true;
    }

    bool CheckThirdSidePartialBlock(List<Vector2Int> area, int width, int height)
    {
        int partialBlocks = 0;
        
        // Check the third side (not fully closed) to see if it's partially blocked
        foreach (Vector2Int cell in area)
        {
            if (cell.x > 1 && grid[cell.x - 2, cell.y]) partialBlocks++;
            if (cell.x < roomWidth - 2 && grid[cell.x + 2, cell.y]) partialBlocks++;
            if (cell.y > 1 && grid[cell.x, cell.y - 2]) partialBlocks++;
            if (cell.y < roomHeight - 2 && grid[cell.x, cell.y + 2]) partialBlocks++;
        }

        return partialBlocks >= 2;
    }

    List<List<Vector2Int>> DetectSpaciousAreas()
    {
        spaciousAreas = new List<List<Vector2Int>>();
        bool[,] visited = new bool[roomWidth, roomHeight];

        // Loop over the grid and check for minimum 3x3 areas and larger
        for (int x = 0; x < roomWidth - 2; x++)
        {
            for (int y = 0; y < roomHeight - 2; y++)
            {
                // If this cell isn't visited and isn't a platform/wall
                if (!visited[x, y] && !grid[x, y])
                {
                    // Try expanding the area to find the largest valid one
                    List<Vector2Int> largestSpaciousArea = FindLargestSpaciousArea(x, y, visited);
                    if (largestSpaciousArea != null && largestSpaciousArea.Count >= 9)  // Must be at least 3x3
                    {
                        spaciousAreas.Add(largestSpaciousArea);
                    }
                }
            }
        }

        return spaciousAreas;
    }

    List<Vector2Int> FindLargestSpaciousArea(int startX, int startY, bool[,] visited)
    {
        List<Vector2Int> bestArea = null;
        int maxWidth = roomWidth - startX;
        int maxHeight = roomHeight - startY;

        // Try different widths and heights (starting from 3x3)
        for (int width = 3; width <= maxWidth; width++)
        {
            for (int height = 3; height <= maxHeight; height++)
            {
                List<Vector2Int> area = GetAreaCells(startX, startY, width, height);

                // Check if this area satisfies the spaciousness condition
                if (area.Count >= 9)  // Must be at least 3x3
                {
                    int surroundingBlocks = CountSurroundingBlocks(area);
                    int maxPossibleBlocks = CalculateMaxSurroundingBlocks(area, width, height);

                    if (surroundingBlocks <= maxPossibleBlocks / 2)
                    {
                        // This area is valid, so it becomes the best area found so far
                        bestArea = new List<Vector2Int>(area);
                    }
                    else
                    {
                        // If a larger area doesn't satisfy the condition, stop checking further heights for this width
                        break;
                    }
                }
            }
        }

        // Only mark the cells in the best area as visited after confirming the area
        if (bestArea != null)
        {
            MarkCellsAsVisited(bestArea, visited);
        }

        return bestArea;
    }

    void MarkCellsAsVisited(List<Vector2Int> area, bool[,] visited)
    {
        foreach (Vector2Int cell in area)
        {
            visited[cell.x, cell.y] = true;
        }
    }
    
    // Count the number of surrounding blocks around the area
    int CountSurroundingBlocks(List<Vector2Int> area)
    {
        int count = 0;

        foreach (Vector2Int cell in area)
        {
            // Check the four directions (up, down, left, right) around each cell
            if (cell.x > 0 && grid[cell.x - 1, cell.y]) count++;
            if (cell.x < roomWidth - 1 && grid[cell.x + 1, cell.y]) count++;
            if (cell.y > 0 && grid[cell.x, cell.y - 1]) count++;
            if (cell.y < roomHeight - 1 && grid[cell.x, cell.y + 1]) count++;
        }

        return count;
    }

    // Calculate the maximum possible surrounding blocks for a given area
    int CalculateMaxSurroundingBlocks(List<Vector2Int> area, int width, int height)
    {
        // Surrounding blocks of the rectangular region
        return 2 * (width + height);  // Sum of the width and height * 2 (for the perimeter)
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
    
    #region Gizmos
    private int currentSpaciousAreaIndex = 0;
    private float nextAreaTime = 0f;
    private float displayDuration = 2f; // Show each area for 5 seconds
    
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
            foreach (List<Vector2Int> lane in room.horizontalLanes)
            {
                foreach (Vector2Int cell in lane)
                {
                    Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                    Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                }
            }
        }

        // Draw Vertical Lanes
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

        // Draw Spacious Areas
        // Draw Spacious Areas (cycling through them one at a time)
        if (showSpaciousAreas && room.spaciousAreas.Count > 0)
        {
            Gizmos.color = Color.magenta; // Semi-transparent magenta

            // Draw only the current spacious area
            List<Vector2Int> currentArea = room.spaciousAreas[room.currentSpaciousAreaIndex];
            foreach (Vector2Int cell in currentArea)
            {
                Vector3 pos = roomBasePosition + new Vector3(cell.x, cell.y, 0) - offset;
                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
            }
        }
        
        // Spacious areas end
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
        
        if (showFreeAreas)
        {
            Gizmos.color = Color.green; // Green for free areas
            for (int x = 0; x < room.roomWidth; x++)
            {
                for (int y = 0; y < room.roomHeight; y++)
                {
                    if (!room.grid[x, y]) // If the cell is "free"
                    {
                        Vector3 pos = roomBasePosition + new Vector3(x, y, 0) - offset;
                        Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                    }
                }
            }
        }
        
        if (showOccupiedAreas)
        {
            Gizmos.color = Color.cyan; // Green for free areas
            for (int x = 0; x < room.roomWidth; x++)
            {
                for (int y = 0; y < room.roomHeight; y++)
                {
                    if (room.grid[x, y]) // If the cell is "free"
                    {
                        Vector3 pos = roomBasePosition + new Vector3(x, y, 0) - offset;
                        Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
                    }
                }
            }
        }
    }
    #endregion
    
    void Update()
    {
        // Cycle to the next area if the display duration has passed
        if (Time.time > nextAreaTime && spaciousAreas != null && spaciousAreas.Count > 0)
        {
            currentSpaciousAreaIndex = (currentSpaciousAreaIndex + 1) % spaciousAreas.Count;
            nextAreaTime = Time.time + displayDuration; // Reset the timer for the next area
        }
    }
    
    
}
