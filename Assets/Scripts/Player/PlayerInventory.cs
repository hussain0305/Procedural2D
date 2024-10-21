using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static event Action<int> OnCoinAmountChanged;
    private int coins;

    private void Start()
    {
        FetchCoins();
    }

    void OnEnable()
    {
        SaveManager.OnSaveFileLoaded += SaveFileLoaded;
    }

    void OnDisable()
    {
        SaveManager.OnSaveFileLoaded -= SaveFileLoaded;
    }

    public void SaveFileLoaded()
    {
        FetchCoins();
    }

    public void FetchCoins()
    {
        coins = SaveManager.GetStartingCoin();
    }
    
    public void SpendCoin(int amount)
    {
        coins -= amount;
        BroadcastCoinChange();
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        BroadcastCoinChange();
    }
    
    private void BroadcastCoinChange()
    {
        OnCoinAmountChanged?.Invoke(coins);
    }
    
    public int GetCoinsBalance()
    {
        return coins;
    }
}
