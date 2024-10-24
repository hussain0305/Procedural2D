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
    MinDimensions
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
}