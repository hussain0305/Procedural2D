using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : Damageable
{
    public EnemyAI EnemyAI;
    public EnemyData.EnemyInfo enemyInfo;
    protected bool isPursuingPlayer;
    protected Transform player;
    protected Vector3 targetPoint;

    [HideInInspector]
    public int damage;

    private IMovementBehavior patrol;
    private IPursuitBehavior edgeBoundPursuit;
    private IAttackBehavior rangedAttack;
    public void Init(Vector3 pointA, Vector3 pointB, float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab)
    {
        // patrol = new PatrolBehavior(transform, pointA, pointB, patrolSpeed);
        // edgeBoundPursuit = new EdgeBoundPursuit(transform, player.transform, pursuitSpeed);
        // rangedAttack = new RangedAttack(transform, player.transform, attackRange, bulletPrefab);

        // enemy.Initialize(patrol, edgeBoundPursuit, rangedAttack);
    }
    
    protected virtual void Update()
    {
        // if (isPursuingPlayer)
        // {
        //     PursuePlayer();
        //     return;
        // }
        // Patrol();
    }

    protected virtual void Patrol()
    {
        // float step = enemyInfo.patrolSpeed * Time.deltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);
        //
        // if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        // {
        //     // targetPoint = targetPoint == enemyInfo.pointA.position ? enemyInfo.pointB.position : enemyInfo.pointA.position;
        // }
    }

    protected virtual void PursuePlayer()
    {
        // float step = enemyInfo.pursueSpeed * Time.deltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, player.position, step);
    }

    public virtual void SpotPlayer(Transform playerTransform)
    {
        // player = playerTransform;
        // isPursuingPlayer = true;
    }

    public virtual void LosePlayer()
    {
        // isPursuingPlayer = false;
        // targetPoint = enemyInfo.pointA.position;
    }

    public virtual void AttackPlayer()
    {
        // To be overridden by specific enemy types for melee or ranged attacks
    }
    
    public override void TakeDamage(int damageAmount)
    {
        // base.TakeDamage(damageAmount);
    }
}
