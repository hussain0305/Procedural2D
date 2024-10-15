using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : Weapon
{
    public float hitDistance = 1f;
    
    public override void PerformPrimaryAttack(Vector2 lastDirection)
    {
        if (IsInCooldown())
        {
            return;
        }

        lastUsedAt = Time.time;
        
        Vector2 rayOrigin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, lastDirection, hitDistance, GlobalData.Instance.groundLayer);
        Debug.DrawRay(rayOrigin, lastDirection * hitDistance, Color.red, 1f);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Damageable>()?.DealDamage(damage);
        }
    }
}
