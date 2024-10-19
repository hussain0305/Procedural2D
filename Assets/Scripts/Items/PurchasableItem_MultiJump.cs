using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableItem_MultiJump : PurchasableItem
{
    public override void WasPurchased()
    {
        GameManager.Instance.player.IncrementNumJumps();
        PostPurchase();
    }
}
