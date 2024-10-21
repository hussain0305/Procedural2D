using System;
using System.Collections;
using UnityEngine;

public static class SaveManager
{
    private static SaveData currentSaveData;
    public static event Action OnSaveFileLoaded;
    
    static SaveManager()
    {
        KeyManager.GenerateAndStoreKeys();
        LoadData();
    }

    public static int GetHealthLevel()
    {
        return currentSaveData.healthLevel;
    }

    public static void SetHealthLevel(int healthLevel)
    {
        currentSaveData.healthLevel = healthLevel;
        SaveData();
    }

    public static int GetStartingCoin()
    {
        return currentSaveData.startingCoin;
    }

    public static void SetStartingCoin(int startingCoin)
    {
        currentSaveData.startingCoin = startingCoin;
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

        if (GameManager.Instance)
        {
            GameManager.Instance.StartCoroutine(DelayedSaveFileLoadedEvent());
        }
        else if (MainMenu.Instance)
        {
            MainMenu.Instance.StartCoroutine(DelayedSaveFileLoadedEvent());
        }
    }
    
    private static System.Collections.IEnumerator DelayedSaveFileLoadedEvent()
    {
        yield return new WaitForSeconds(0.1f);
        OnSaveFileLoaded?.Invoke();
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