using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomZoneFilterFactory
{
    public static Func<List<Vector2Int>, bool> GetFilter(RoomZoneFilterType filterType, params object[] parameters)
    {
        int minWidth;
        switch (filterType)
        {
            case RoomZoneFilterType.AttachedToWall:
                return RoomZoneFilters.AttachedToWall();
            case RoomZoneFilterType.AttachedToCeiling:
                return RoomZoneFilters.AttachedToCeiling();
            case RoomZoneFilterType.AttachedToFloor:
                return RoomZoneFilters.AttachedToFloor();
            case RoomZoneFilterType.MinWidth:
                minWidth = (int)parameters[0];
                return RoomZoneFilters.MinWidth(minWidth);
            case RoomZoneFilterType.MinHeight:
                int minHeight = (int)parameters[0];
                return RoomZoneFilters.MinHeight(minHeight);
            case RoomZoneFilterType.MinDistanceFromGround:
                int minDistanceFromGround = (int)parameters[0];
                return RoomZoneFilters.MinDistanceFromGround(minDistanceFromGround);
            case RoomZoneFilterType.MinDistanceFromCeiling:
                int minDistanceFromCeiling = (int)parameters[0];
                return RoomZoneFilters.MinDistanceFromCeiling(minDistanceFromCeiling);
            case RoomZoneFilterType.MinDimensions:
                Vector2Int minDimensions = (Vector2Int)parameters[0];
                return RoomZoneFilters.MinDimensions(minDimensions);
            case RoomZoneFilterType.OnAPlatform:
                minWidth = (int)parameters[0];
                return RoomZoneFilters.FindSuitablePlatform(minWidth);
            default:
                throw new ArgumentException("Invalid filter type");
        }
    }
}
