using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrefab;

    public override void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= enemyData.attackRange)
        {
            // Perform ranged attack
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }
}