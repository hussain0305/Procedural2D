using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/TrapData", order = 1)]
public class TrapData : ScriptableObject
{
    [System.Serializable]
    public struct TrapFilter
    {
        public RoomZoneFilterType filterType;
        public string filterParam;
    }

    [System.Serializable]
    public struct TrapInfo
    {
        public TrapType trapType;
        public GameObject trapPrefab;
        public BulletType bulletType;
        public GameObject bulletPrefab;
        public List<TrapFilter> filters;
        public int damage;
    }

    public List<TrapInfo> traps;
    private Dictionary<TrapType, TrapInfo> trapDataDictionary;

    private static TrapData _instance;
    public static TrapData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<TrapData>("TrapData");
            }

            return _instance;
        }
    }

    public void EnsureDictionaryReady()
    {
        if (trapDataDictionary != null)
        {
            return;
        }
        trapDataDictionary = new Dictionary<TrapType, TrapInfo>();
        foreach (TrapInfo trapInfo in traps)
        {
            trapDataDictionary.Add(trapInfo.trapType, trapInfo);
        }
    }

    public bool GetTrapInfo(TrapType trapType, out TrapInfo trapInfo)
    {
        EnsureDictionaryReady();
        if (trapDataDictionary.TryGetValue(trapType, out var value))
        {
            trapInfo = value;
            return true;
        }

        trapInfo = new TrapInfo();
        return false;
    }

    public List<Func<List<Vector2Int>, bool>> GetFiltersForTrap(TrapType trapType)
    {
        EnsureDictionaryReady();
        if (trapDataDictionary.TryGetValue(trapType, out TrapInfo trapInfo))
        {
            List<Func<List<Vector2Int>, bool>> filters = new List<Func<List<Vector2Int>, bool>>();

            foreach (var filter in trapInfo.filters)
            {
                filters.Add(CreateFilter(filter));
            }
            return filters;
        }
        return null;
    }

    private Func<List<Vector2Int>, bool> CreateFilter(TrapFilter trapFilter)
    {
        switch (trapFilter.filterType)
        {
            case RoomZoneFilterType.AttachedToWall:
                return RoomZoneFilters.AttachedToWall();
            case RoomZoneFilterType.AttachedToCeiling:
                return RoomZoneFilters.AttachedToCeiling();
            case RoomZoneFilterType.MinWidth:
                int minWidth = int.Parse(trapFilter.filterParam);
                return RoomZoneFilters.MinWidth(minWidth);
            case RoomZoneFilterType.MinHeight:
                int minHeight = int.Parse(trapFilter.filterParam);
                return RoomZoneFilters.MinHeight(minHeight);
            case RoomZoneFilterType.MinDistanceFromGround:
                int minDistanceFromGround = int.Parse(trapFilter.filterParam);
                return RoomZoneFilters.MinDistanceFromGround(minDistanceFromGround);
            case RoomZoneFilterType.MinDistanceFromCeiling:
                int minDistanceFromCeiling = int.Parse(trapFilter.filterParam);
                return RoomZoneFilters.MinDistanceFromCeiling(minDistanceFromCeiling);
            case RoomZoneFilterType.MinDimensions:
                Vector2Int minDimensions = ParseVector2Int(trapFilter.filterParam);
                return RoomZoneFilters.MinDimensions(minDimensions);
            default:
                throw new ArgumentException("Invalid filter type");
        }
    }

    private Vector2Int ParseVector2Int(string param)
    {
        string[] parts = param.Trim('(', ')').Split(',');
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);
        return new Vector2Int(x, y);
    }
}
