using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : IAttackBehavior
{
    private Transform enemy;
    private Transform target;
    private float range;
    private GameObject projectilePrefab;

    public RangedAttack(Transform enemy, Transform target, float range, GameObject projectilePrefab)
    {
        this.enemy = enemy;
        this.target = target;
        this.range = range;
        this.projectilePrefab = projectilePrefab;
    }

    public void Execute()
    {
        // Implement straight-line ranged attack logic
    }
}
