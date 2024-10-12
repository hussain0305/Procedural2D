using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabPickup : Pickup
{
    public override void WasPickedUp(PlayerController player)
    {
        player.abilities.hasWallGrab = true;
        base.WasPickedUp(player);
    }
}
