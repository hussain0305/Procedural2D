using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerAI : EnemyAI
{
    public override void Init(Transform enemyTransform, EnemyType enemyType, System.Action onEdgeDetected)
    {
        EnemyData.Instance.GetEnemyInfo(enemyType, out EnemyData.EnemyInfo enemyInfo);

        movementBehavior = new PatrolBehavior(enemyTransform, enemyInfo.patrolSpeed, onEdgeDetected);
        pursuitBehavior = new EdgeBoundPursuit(enemyTransform, GameManager.Instance.player.transform, enemyInfo.pursueSpeed);
        attackBehavior = new RangedAttack(
            enemyTransform,
            GameManager.Instance.player.transform,
            enemyInfo.attackRange,
            enemyInfo.bulletPrefab,
            enemyInfo.viewDistance,
            enemyInfo.viewAngle
        );
    }
}
