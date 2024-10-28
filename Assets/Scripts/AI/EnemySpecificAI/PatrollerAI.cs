using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerAI : EnemyAI
{
    public override void Init(Transform enemyTransform, float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab,
        Action onEdgeDetected)
    {
        movementBehavior = new PatrolBehavior(enemyTransform, patrolSpeed, onEdgeDetected);
        pursuitBehavior = new EdgeBoundPursuit(enemyTransform, GameManager.Instance.player.transform, pursuitSpeed);
        attackBehavior = new RangedAttack(enemyTransform, GameManager.Instance.player.transform, attackRange, bulletPrefab);
    }
}
