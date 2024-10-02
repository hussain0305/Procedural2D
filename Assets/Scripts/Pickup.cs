using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.GetComponentInParent<PlayerController>())
        {
            WasPickedUp(other.gameObject.GetComponentInParent<PlayerController>());
        }
    }
    
    public virtual void WasPickedUp(PlayerController player)
    {
        Destroy(gameObject);
    }
}
