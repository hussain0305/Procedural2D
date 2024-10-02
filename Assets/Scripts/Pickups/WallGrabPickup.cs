using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabPickup : Pickup
{
    public override void WasPickedUp(PlayerController player)
    {
        player.GetComponent<Collider2D>().sharedMaterial = GlobalData.Instance.wallGrabMaterial;
        base.WasPickedUp(player);
    }
}
