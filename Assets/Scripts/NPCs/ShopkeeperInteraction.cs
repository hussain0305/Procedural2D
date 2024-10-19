using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperInteraction : NPCInteraction
{
    public void Interaction_Talk()
    {
        Debug.Log("Talked with the Shopkeeper");
    }
    
    public void Interaction_Shop()
    {
        Debug.Log("Shopped with the Shopkeeper");
    }
}
