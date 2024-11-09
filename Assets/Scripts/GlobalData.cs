using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlobalData", menuName = "ScriptableObjects/GlobalData", order = 1)]
public class GlobalData : ScriptableObject
{
    private static GlobalData _instance;

    public static GlobalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GlobalData>("GlobalData");
                if (_instance == null)
                {
                    Debug.LogError("GlobalData instance not found in Resources.");
                }
            }
            return _instance;
        }
    }
    
    public RoomColors[] roomColors;

    [Header("Collision Layers")] 
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask ceilingLayer;
    public LayerMask roomVolumeLayer;
    public LayerMask playerLayer;

    [Header("Materials")]
    public PhysicsMaterial2D wallSlideMaterial;
    public PhysicsMaterial2D wallGrabMaterial;
    
    [Header("Pickups")]
    public PickupPrefabs[] pickupPrefabs;

    [Header("Tiles")]
    public RuleTile wallTile;
    public RuleTile platformTile;

    private Dictionary<RoomType, Material> roomBackgroundDictionary;
    public Material GetRoomColor(RoomType roomType)
    {
        if (roomBackgroundDictionary == null)
        {
            roomBackgroundDictionary = new Dictionary<RoomType, Material>();
            foreach (RoomColors roomColor in roomColors)
            {
                roomBackgroundDictionary.TryAdd(roomColor.roomtype, roomColor.material);
            }
        }
        return roomBackgroundDictionary[roomType];
    }

    private Dictionary<PickupType, GameObject> pickupPrefabDictionary;
    public GameObject GetPickupPrefab(PickupType pickupType)
    {
        if (pickupPrefabDictionary == null)
        {
            pickupPrefabDictionary = new Dictionary<PickupType, GameObject>();
            foreach (PickupPrefabs pickupPrefab in pickupPrefabs)
            {
                pickupPrefabDictionary.Add(pickupPrefab.pickupType, pickupPrefab.prefab);
            }
        }

        if (pickupPrefabDictionary.ContainsKey(pickupType))
        {
            return pickupPrefabDictionary[pickupType];
        }
        return null;
    }
}