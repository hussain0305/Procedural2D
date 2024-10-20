using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Shop : Menu<PurchasableItemInfo>
{
    public PurchasableItems purchasableItems;
    private Dictionary<PurchableItemType, Action> itemActions;

    protected override void Start()
    {
        InitShop();
        base.Start();
    }

    private void InitShop()
    {
        itemActions = new Dictionary<PurchableItemType, Action>
        {
            { PurchableItemType.WallGrab, () => GameManager.Instance.player.abilities.hasWallGrab = true },
            { PurchableItemType.MultiJump, () => GameManager.Instance.player.abilities.additionalJumps++ },
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
            purchaseAction.Invoke();
        }
    }
}
