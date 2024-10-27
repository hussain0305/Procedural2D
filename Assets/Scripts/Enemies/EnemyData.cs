using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public struct EnemyInfo
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
        public List<PositioningFilters> filters;
        public int health;
        public float patrolSpeed;
        public float patrolPause;
        public float pursueSpeed;
        public float attackRange;
        public BulletType bulletType;
        public GameObject bulletPrefab;
        public SpawnRequirement spawnRequirement;
    }

    public List<EnemyInfo> enemies;
    private Dictionary<EnemyType, EnemyInfo> enemyDataDictionary;

    private static EnemyData _instance;
    public static EnemyData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EnemyData>("EnemyData");
            }

            return _instance;
        }
    }

    public void EnsureDictionaryReady()
    {
        if (enemyDataDictionary != null)
        {
            return;
        }
        enemyDataDictionary = new Dictionary<EnemyType, EnemyInfo>();
        foreach (EnemyInfo enemyInfo in enemies)
        {
            enemyDataDictionary.Add(enemyInfo.enemyType, enemyInfo);
        }
    }

    public bool GetEnemyInfo(EnemyType enemyType, out EnemyInfo enemyInfo)
    {
        EnsureDictionaryReady();
        if (enemyDataDictionary.TryGetValue(enemyType, out var value))
        {
            enemyInfo = value;
            return true;
        }

        enemyInfo = new EnemyInfo();
        return false;
    }
    
    public List<Func<List<Vector2Int>, bool>> GetFiltersForEnemy(EnemyType enemyType)
    {
        EnsureDictionaryReady();
        if (enemyDataDictionary.TryGetValue(enemyType, out EnemyInfo trapInfo))
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
