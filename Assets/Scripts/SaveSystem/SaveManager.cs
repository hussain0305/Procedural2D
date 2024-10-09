using UnityEngine;

public static class SaveManager
{
    private static SaveData currentSaveData;

    static SaveManager()
    {
        KeyManager.GenerateAndStoreKeys();
        LoadData();
    }

    public static int GetCurrentHealthLevel()
    {
        return currentSaveData.currentHealthLevel;
    }

    public static void SetCurrentHealthLevel(int healthLevel)
    {
        currentSaveData.currentHealthLevel = healthLevel;
        SaveData();
    }

    private static void LoadData()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            currentSaveData = WebGLSaveSystem.LoadGame();
        }
        else
        {
            currentSaveData = SaveSystem.LoadGame();
        }
    }

    private static void SaveData()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLSaveSystem.SaveGame(currentSaveData);
        }
        else
        {
            SaveSystem.SaveGame(currentSaveData);
        }
    }
}