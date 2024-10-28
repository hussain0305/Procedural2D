using System;
using System.Collections;
using UnityEngine;

public class Enemy : Damageable
{
    [HideInInspector] public Vector2Int locatedInRoom;

    public EnemyAI enemyAI;
    [HideInInspector] public int damage;
    
    protected EnemyData.EnemyInfo enemyInfo;
    protected bool isPursuingPlayer;
    protected Transform Player => GameManager.Instance.player.transform;
    protected float pausePatrolUntil = 0;
    protected bool PatrolPaused => Time.time < pausePatrolUntil;
    
    private readonly float patrolPauseDuration = 3.0f;

    public void Init(float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab, EnemyData.EnemyInfo _enemyInfo)
    {
        enemyInfo = _enemyInfo;
        enemyAI.Init(this.transform, patrolSpeed, pursuitSpeed, attackRange, bulletPrefab, OnEdgeDetected);
    }

    private void Update()
    {
        if (!PatrolPaused)
        {
            enemyAI.UpdateBehavior(isPursuingPlayer);
        }
    }

    public virtual void SpotPlayer(Transform playerTransform)
    {
        isPursuingPlayer = true;
    }

    public virtual void LosePlayer()
    {
        isPursuingPlayer = false;
    }

    public override void TakeDamage(int damageAmount)
    {
        // Handle damage logic
    }

    private void OnEdgeDetected()
    {
        if (!PatrolPaused)
        {
            pausePatrolUntil = Time.time + patrolPauseDuration;
        }
    }
}