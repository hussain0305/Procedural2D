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
        PlayerInventory.OnCoinAmountChanged += UpdateCoinBalance;
    }

    private void OnDestroy()
    {
        PlayerInventory.OnCoinAmountChanged -= UpdateCoinBalance;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateCoinBalance(GameManager.Instance.PlayerInventory.GetCoinsBalance());
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
            currentShopItem.inventoryCountText.text = selectedItem.inventoryCount.ToString();
            availableItems[currentItemIndex] = selectedItem;
            PurchaseItem(selectedItem);
            currentShopItem.Setup(
                selectedItem, 
                selectedItem.itemName, 
                selectedItem.description, 
                selectedItem.itemIcon, 
                selectedItem.price, 
                selectedItem.inventoryCount
            );
            EvaluateMenu();
            // if (selectedItem.inventoryCount == 0)
            // {
            //     currentShopItem.inventorySection.SetActive(false);
            //     currentShopItem.soldOutSection.SetActive(true);
            //     itemAvailability[currentItemIndex] = false;
            //     NavigateMenu(1);
            // }
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

    public void UpdateCoinBalance(int coins)
    {
        coinAmountText.text = coins.ToString();
        EvaluateMenu();
    }
    
    public override void EvaluateMenu()
    {
        if (itemAffordability == null)
        {
            itemAffordability = new Dictionary<int, bool>();
        }
        int playerCoinBalance = GameManager.Instance.PlayerInventory.GetCoinsBalance();
        for (int i = 0; i < availableItems.Count; i++)
        {
            var purchasableItem = (PurchasableItemInfo)(object)availableItems[i]; 
            itemAffordability.TryAdd(i, true);
            itemAffordability[i] = purchasableItem.price <= playerCoinBalance;
            itemAvailability[i] = purchasableItem.inventoryCount > 0;
            MenuItem<PurchasableItemInfo> shopItem = menuItems[i];
            shopItem.UpdateVisibleSections();
        }

        if (currentItemIndex < availableItems.Count)
        {
            if (!itemAffordability[currentItemIndex] || !itemAvailability[currentItemIndex])
            {
                NavigateMenu(1);
            }
        }
    }
}
