using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbedAttack : IAttackBehavior
{
    private Transform enemy;
    private Transform target;
    private GameObject projectilePrefab;

    public LobbedAttack(Transform enemy, Transform target, GameObject projectilePrefab)
    {
        this.enemy = enemy;
        this.target = target;
        this.projectilePrefab = projectilePrefab;
    }

    public void Execute()
    {
        // Implement lobbed attack logic with projectile motion
    }

    public bool CanSeePlayer()
    {
        return true;
    }
}
