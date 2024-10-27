using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum RoomZoneFilterType
{
    AttachedToWall,
    AttachedToCeiling,
    AttachedToFloor,
    MinWidth,
    MinHeight,
    MinDistanceFromGround,
    MinDistanceFromCeiling,
    MinDimensions,
    OnAPlatform
}

public static class RoomZoneFilters
{
    public static Func<List<Vector2Int>, bool> AttachedToWall()
    {
        return (area) => area.Exists(cell => cell.x == 0 || cell.x == Global.CELL_SIZE_INTERIOR_X - 1);
    }

    public static Func<List<Vector2Int>, bool> AttachedToCeiling()
    {
        return (area) => area.Exists(cell => cell.y == Global.CELL_SIZE_INTERIOR_Y - 1);
    }

    public static Func<List<Vector2Int>, bool> AttachedToFloor()
    {
        return (area) => area.Exists(cell => cell.y == 0);
    }

    public static Func<List<Vector2Int>, bool> MinWidth(int minWidth)
    {
        return (area) =>
        {
            if (area == null || area.Count == 0)
            {
                return false;
            }

            return GetAreaDimensions(area).x >= minWidth;
        };
    }

    public static Func<List<Vector2Int>, bool> MinHeight(int minHeight)
    {
        return (area) =>
        {
            if (area == null || area.Count == 0)
            {
                return false;
            }

            return GetAreaDimensions(area).y >= minHeight;
        };
    }
    
    public static Func<List<Vector2Int>, bool> MinDistanceFromGround(int minDistanceFromGround)
    {
        return (area) =>
        {
            if (area == null || area.Count == 0)
            {
                return false;
            }
            return area.All(cell => cell.y >= minDistanceFromGround);
        };
    }

    public static Func<List<Vector2Int>, bool> MinDistanceFromCeiling(int minDistanceFromCeiling)
    {
        return (area) =>
        {
            if (area == null || area.Count == 0)
            {
                return false;
            }
            return area.All(cell => cell.y <= Global.CELL_SIZE_INTERIOR_Y - 1 - minDistanceFromCeiling);
        };
    }

    public static Func<List<Vector2Int>, bool> MinDimensions(Vector2Int minDimensions)
    {
        return (area) =>
        {
            if (area == null || area.Count == 0)
            {
                return false;
            }
        
            Vector2Int areaDimensions = GetAreaDimensions(area);
            return areaDimensions.x >= minDimensions.x && areaDimensions.y >= minDimensions.y;
        };
    }

    public static Func<List<Vector2Int>, bool> FindSuitablePlatform(int minWidth)
    {
        return (platform) =>
        {
            int topY = platform.Max(block => block.y);
            var topRow = platform.Where(block => block.y == topY).ToList();
            int topRowWidth = topRow.Max(block => block.x) - topRow.Min(block => block.x) + 1;
            return topRowWidth >= minWidth && topY < Global.CELL_SIZE_INTERIOR_Y - 2;
        };
    }
    
    public static Vector2Int GetAreaDimensions(List<Vector2Int> area)
    {
        if (area == null || area.Count == 0)
        {
            return Vector2Int.zero;
        }

        int minX = area.Min(cell => cell.x);
        int minY = area.Min(cell => cell.y);
        int maxX = area.Max(cell => cell.x);
        int maxY = area.Max(cell => cell.y);

        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    public static Func<List<Vector2Int>, bool> Combine(params Func<List<Vector2Int>, bool>[] filters)
    {
        return (area) =>
        {
            foreach (var filter in filters)
            {
                if (!filter(area))
                {
                    return false;
                }
            }
            return true;
        };
    }
    
    public static Func<List<Vector2Int>, bool> CreateFilter(PositioningFilters positionalFilter)
    {
        int minWidth;
        switch (positionalFilter.filterType)
        {
            case RoomZoneFilterType.AttachedToWall:
                return RoomZoneFilters.AttachedToWall();
            case RoomZoneFilterType.AttachedToCeiling:
                return RoomZoneFilters.AttachedToCeiling();
            case RoomZoneFilterType.MinWidth:
                minWidth = int.Parse(positionalFilter.filterParam);
                return RoomZoneFilters.MinWidth(minWidth);
            case RoomZoneFilterType.MinHeight:
                int minHeight = int.Parse(positionalFilter.filterParam);
                return RoomZoneFilters.MinHeight(minHeight);
            case RoomZoneFilterType.MinDistanceFromGround:
                int minDistanceFromGround = int.Parse(positionalFilter.filterParam);
                return RoomZoneFilters.MinDistanceFromGround(minDistanceFromGround);
            case RoomZoneFilterType.MinDistanceFromCeiling:
                int minDistanceFromCeiling = int.Parse(positionalFilter.filterParam);
                return RoomZoneFilters.MinDistanceFromCeiling(minDistanceFromCeiling);
            case RoomZoneFilterType.MinDimensions:
                Vector2Int minDimensions = ParseVector2Int(positionalFilter.filterParam);
                return RoomZoneFilters.MinDimensions(minDimensions);
            case RoomZoneFilterType.OnAPlatform:
                minWidth = int.Parse(positionalFilter.filterParam);
                return RoomZoneFilters.FindSuitablePlatform(minWidth);
            default:
                throw new ArgumentException("Invalid filter type");
        }
    }

    private static Vector2Int ParseVector2Int(string param)
    {
        string[] parts = param.Trim('(', ')').Split(',');
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);
        return new Vector2Int(x, y);
    }

}