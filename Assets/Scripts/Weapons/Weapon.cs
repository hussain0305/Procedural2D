using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float cooldown = 2f;

    protected float lastUsedAt = 0;
    
    public virtual void PerformPrimaryAttack()
    {
        
    }
    public virtual void PerformPrimaryAttack(Vector2 lastDirection)
    {
        
    }

    public bool IsInCooldown()
    {
        return Time.time < lastUsedAt + cooldown;
    }
}
