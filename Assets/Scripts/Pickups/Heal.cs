using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Pickup
{
    public override void WasPickedUp(PlayerController player)
    {
        
    }

    public override void WasPickedUp(PlayerAttributes player)
    {
        player.Heal(20);
    }
}
