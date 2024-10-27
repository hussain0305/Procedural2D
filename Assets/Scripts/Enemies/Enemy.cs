using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    public EnemyAI enemyAI;
    public EnemyData.EnemyInfo enemyInfo;
    protected bool isPursuingPlayer;

    protected Transform Player
    {
        get
        {
            return GameManager.Instance.player.transform;
        }
    }
    protected Vector3 targetPoint;

    [HideInInspector]
    public int damage;

    private IMovementBehavior patrol;
    private IPursuitBehavior edgeBoundPursuit;
    private IAttackBehavior rangedAttack;

    public void Init(Vector3 pointA, Vector3 pointB, float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab)
    {
        patrol = new PatrolBehavior(transform, pointA, pointB, patrolSpeed, 3);
        edgeBoundPursuit = new EdgeBoundPursuit(transform, Player.transform, pursuitSpeed);
        rangedAttack = new RangedAttack(transform, Player.transform, attackRange, bulletPrefab);

        enemyAI.Init(patrol, edgeBoundPursuit, rangedAttack);
    }
    
    protected virtual void Update()
    {
        if (isPursuingPlayer)
        {
            PursuePlayer();
            return;
        }
        Patrol();
    }

    protected virtual void Patrol()
    {
        patrol.Execute();
    }

    protected virtual void PursuePlayer()
    {
        edgeBoundPursuit.Execute();
    }

    public virtual void SpotPlayer(Transform playerTransform)
    {
        isPursuingPlayer = true;
    }

    public virtual void LosePlayer()
    {
        isPursuingPlayer = false;
    }

    public virtual void AttackPlayer()
    {
        rangedAttack.Execute();
    }
    
    public override void TakeDamage(int damageAmount)
    {
        // base.TakeDamage(damageAmount);
    }
}