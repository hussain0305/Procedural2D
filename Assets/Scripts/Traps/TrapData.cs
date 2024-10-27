using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/TrapData", order = 1)]
public class TrapData : ScriptableObject
{
    [System.Serializable]
    public struct TrapInfo
    {
        public TrapType trapType;
        public GameObject trapPrefab;
        public BulletType bulletType;
        public GameObject bulletPrefab;
        public List<PositioningFilters> filters;
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
                filters.Add(RoomZoneFilters.CreateFilter(filter));
            }
            return filters;
        }
        return null;
    }
}
