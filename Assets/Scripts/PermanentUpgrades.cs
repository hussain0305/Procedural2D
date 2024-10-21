using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PermanentUpgrades : MonoBehaviour
{
    public GameObject upgradeSection;
    public Button upgradeSectionButton;
    public Button upgradeMaxHealth;
    public Button upgradeStartingCoins;
    public Button addCoins;

    private void Awake()
    {
        upgradeSectionButton.onClick.AddListener(ToggleSectionVisibility);
        upgradeMaxHealth.onClick.AddListener(UpgradeMaxHealth);
        upgradeStartingCoins.onClick.AddListener(UpgradeStartingCoins);
        addCoins.onClick.AddListener(AddCoins);
    }

    public void ToggleSectionVisibility()
    {
        upgradeSection.SetActive(!upgradeSection.activeSelf);
    }

    public void UpgradeMaxHealth()
    {
        SaveManager.SetHealthLevel(SaveManager.GetHealthLevel() + 10);
    }

    public void UpgradeStartingCoins()
    {
        SaveManager.SetStartingCoin(SaveManager.GetStartingCoin() + 10);
    }
    
    public void AddCoins()
    {
        GameManager.Instance.PlayerInventory.AddCoins(10);
    }
}
