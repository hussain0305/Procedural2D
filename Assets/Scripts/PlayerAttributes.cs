using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public Healthbar healthbar;
    //Health
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int healAmount;

    private int currentHealth;

    public event Action<int, int> OnHealthChanged;  
    
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = Mathf.Max(0, value);
                TriggerHealthChanged();
            }
        }
    }
    
    public void Start()
    {
        InitHealth();
    }

    public void InitHealth()
    {
        maxHealth = SaveManager.GetCurrentHealthLevel();
        CurrentHealth = maxHealth - 50;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            //Death
        }
    }
    
    public void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    private void TriggerHealthChanged()
    {
        if (OnHealthChanged != null)
        {
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }
    }
}
