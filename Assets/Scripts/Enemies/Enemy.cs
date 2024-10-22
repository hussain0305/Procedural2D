using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    public EnemyData enemyData;
    protected bool isPursuingPlayer;
    protected Transform player;
    protected Vector3 targetPoint;

    protected virtual void Start()
    {
        targetPoint = enemyData.pointA.position;
    }

    protected virtual void Update()
    {
        Patrol();
        if (isPursuingPlayer)
        {
            PursuePlayer();
        }
    }

    protected virtual void Patrol()
    {
        // Move between points A and B
        float step = enemyData.patrolSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = targetPoint == enemyData.pointA.position ? enemyData.pointB.position : enemyData.pointA.position;
        }
    }

    protected virtual void PursuePlayer()
    {
        float step = enemyData.pursueSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);
    }

    public virtual void SpotPlayer(Transform playerTransform)
    {
        player = playerTransform;
        isPursuingPlayer = true;
    }

    public virtual void LosePlayer()
    {
        isPursuingPlayer = false;
        targetPoint = enemyData.pointA.position;
    }

    public virtual void AttackPlayer()
    {
        // To be overridden by specific enemy types for melee or ranged attacks
    }
    
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
    }
}
