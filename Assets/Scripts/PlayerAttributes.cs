using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    //Health
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int currentHealth;
    [HideInInspector]
    public int healAmount;

    public void Start()
    {
        InitHealth();
    }

    public void InitHealth()
    {
        currentHealth = SaveManager.GetCurrentHealthLevel();
        Debug.Log("Current health: " + currentHealth);
    }
}
