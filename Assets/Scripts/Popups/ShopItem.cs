using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MenuItem<PurchasableItemInfo>
{
    public override void UpdateVisibleSections()
    {
        var purchasableItem = (PurchasableItemInfo)(object)itemData; 
        int playerCoins = GameManager.Instance.PlayerInventory.GetCoinsBalance();
        //Inventory section will be active if it's both available and player has enough coins to buy it
        inventorySection.SetActive(purchasableItem.inventoryCount > 0 && playerCoins >= purchasableItem.price);
        //sold out section will be active if it's out of stock
        soldOutSection.SetActive(purchasableItem.inventoryCount <= 0 );
        //not enough coins section will be active if sold out section is not active and player doesn't have enough coins
        notEnoughCoinsSection.SetActive(!soldOutSection.activeSelf && playerCoins < purchasableItem.price);
    }
}