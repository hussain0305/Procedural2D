using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject)
        {
            PlayerController controller = other.gameObject.GetComponentInParent<PlayerController>();
            if (controller)
            {
                WasPickedUp(controller);
            }
            PlayerAttributes attributes = other.gameObject.GetComponentInParent<PlayerAttributes>();
            if (attributes)
            {
                WasPickedUp(attributes);
            }
        }
    }
    
    public virtual void WasPickedUp(PlayerController player)
    {
        Destroy(gameObject);
    }
    
    public virtual void WasPickedUp(PlayerAttributes player)
    {
        
    }
}
