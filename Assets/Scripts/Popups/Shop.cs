using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Shop : Menu<PurchasableItemInfo>
{
    public TextMeshProUGUI coinAmountText;
    public PurchasableItems purchasableItems;
    private Dictionary<PurchableItemType, Action> itemActions;

    protected override void Start()
    {
        InitShop();
        base.Start();
        PlayerInventory.OnCoinAmountChanged += UpdateCoinBalanceOnScreen;
    }

    public void OnEnable()
    {
        base.OnEnable();
        UpdateCoinBalanceOnScreen(GameManager.Instance.PlayerInventory.GetCoinsBalance());
    }
    
    private void InitShop()
    {
        
        itemActions = new Dictionary<PurchableItemType, Action>
        {
            { PurchableItemType.WallGrab, () => GameManager.Instance.GivePlayerWallGrab() },
            { PurchableItemType.MultiJump, () => GameManager.Instance.GivePlayerAdditionalJump() },
            { PurchableItemType.Dash, () => GameManager.Instance.GivePlayerDash() },
        };

        availableItems = new List<PurchasableItemInfo>(purchasableItems.items);
        itemAvailability = new Dictionary<int, bool>();
        for (int i = 0; i < availableItems.Count; i++)
        {
            itemAvailability.Add(i, true);
        }
    }

    protected override void SetupMenuItem(MenuItem<PurchasableItemInfo> menuItem, PurchasableItemInfo item)
    {
        menuItem.Setup(item, item.itemName, item.description, item.itemIcon, item.price, item.inventoryCount);
    }

    protected override void PurchaseSelectedItem()
    {
        PurchasableItemInfo selectedItem = availableItems[currentItemIndex];

        if (selectedItem.inventoryCount > 0)
        {
            selectedItem.inventoryCount--;

            MenuItem<PurchasableItemInfo> currentShopItem = menuItems[currentItemIndex];
            currentShopItem.inventoryCount.text = selectedItem.inventoryCount.ToString();
            availableItems[currentItemIndex] = selectedItem;
            PurchaseItem(selectedItem);

            if (selectedItem.inventoryCount == 0)
            {
                currentShopItem.inventorySection.SetActive(false);
                currentShopItem.soldOutSection.SetActive(true);
                itemAvailability[currentItemIndex] = false;
                NavigateMenu(1);
            }
        }
    }

    private void PurchaseItem(PurchasableItemInfo item)
    {
        if (itemActions.TryGetValue(item.itemType, out Action purchaseAction))
        {
            GameManager.Instance.PlayerInventory.SpendCoin(item.price);
            purchaseAction.Invoke();
        }
    }

    public void UpdateCoinBalanceOnScreen(int coins)
    {
        coinAmountText.text = coins.ToString();
    }
}
