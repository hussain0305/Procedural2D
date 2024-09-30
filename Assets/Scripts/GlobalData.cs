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

    public Color GetRoomColor(RoomType roomType)
    {
        foreach (RoomColors roomColor in roomColors)
        {
            if (roomColor.roomtype == roomType)
            {
                return roomColor.color;
            }
        }
        return Color.black;
    }
}