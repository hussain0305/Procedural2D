using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentHealthLevel = 100;
    public int playerScore;
    public Vector3 playerPosition;
}

public static class SaveSystem
{
    private static string GetSavePath()
    {
        return Application.persistentDataPath + "/savefile.json";
    }

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        string encryptedJson = EncryptionUtils.Encrypt(json);
        
        File.WriteAllText(GetSavePath(), encryptedJson);
    }

    public static SaveData LoadGame()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string encryptedJson = File.ReadAllText(path);
            string json = EncryptionUtils.Decrypt(encryptedJson);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found, returning default data.");
            return new SaveData();
        }
    }
}