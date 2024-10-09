using UnityEngine;

public static class WebGLSaveSystem
{
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        string encryptedJson = EncryptionUtils.Encrypt(json);
        PlayerPrefs.SetString("SaveData", encryptedJson);
        PlayerPrefs.Save();
    }

    public static SaveData LoadGame()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string encryptedJson = PlayerPrefs.GetString("SaveData");
            string json = EncryptionUtils.Decrypt(encryptedJson);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("No save data found in PlayerPrefs, returning default values.");
            return new SaveData();  // Return default data for first-time play
        }
    }
}