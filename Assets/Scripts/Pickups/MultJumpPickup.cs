using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultJumpPickup : Pickup
{
    public override void WasPickedUp(PlayerController player)
    {
        player.IncrementNumJumps();
        base.WasPickedUp(player);
    }
}
