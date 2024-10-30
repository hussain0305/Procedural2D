using UnityEngine;

public class PatrollerAI : EnemyAI
{
    public override void Init(Transform enemyTransform, EnemyType enemyType, System.Action onEdgeDetected, System.Action onPlayerDetected, System.Action onPlayerLost)
    {
        EnemyData.Instance.GetEnemyInfo(enemyType, out EnemyData.EnemyInfo enemyInfo);

        movementBehavior = new PatrolBehavior(enemyTransform, enemyInfo.patrolSpeed, onEdgeDetected);
        pursuitBehavior = new EdgeBoundPursuit(enemyTransform, GameManager.Instance.player.transform, enemyInfo.pursueSpeed);
        attackBehavior = new RangedAttack(
            enemyTransform,
            GameManager.Instance.player.transform,
            enemyInfo.attackRange,
            enemyInfo.attackCooldown,
            enemyInfo.bulletPrefab,
            enemyInfo.viewDistance,
            enemyInfo.viewAngle,
            onPlayerDetected,
            onPlayerLost
        );
    }
}
