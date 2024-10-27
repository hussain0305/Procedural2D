using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [HideInInspector]
    public int health;

    public virtual void TakeDamage(int damageAmount)
    {
        
    }
}
