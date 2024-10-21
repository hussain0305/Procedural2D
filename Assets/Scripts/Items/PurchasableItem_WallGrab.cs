using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableItem_WallGrab : PurchasableItem
{
    public override void WasPurchased()
    {
        GameManager.Instance.PlayerController.abilities.hasWallGrab = true;
        PostPurchase();
    }
}
