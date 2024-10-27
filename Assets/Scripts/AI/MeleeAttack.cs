using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : IAttackBehavior
{
    private Transform enemy;
    private float attackRange;

    public MeleeAttack(Transform enemy, float attackRange)
    {
        this.enemy = enemy;
        this.attackRange = attackRange;
    }

    public void Execute()
    {
        // Implement melee attack logic
    }
}
