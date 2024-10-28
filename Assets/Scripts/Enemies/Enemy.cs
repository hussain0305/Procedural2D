using System.Collections;
using UnityEngine;

public class Enemy : Damageable
{
    public EnemyAI enemyAI;
    public bool PatrolPaused => Time.time < pausePatrolUntil;
    [HideInInspector] public int damage;
    [HideInInspector] public Vector2Int locatedInRoom;

    protected EnemyData.EnemyInfo enemyInfo;
    protected bool isPursuingPlayer;
    protected Transform Player => GameManager.Instance.player.transform;

    private float pausePatrolUntil = 0;

    public void Init(EnemyType enemyType)
    {
        EnemyData.Instance.GetEnemyInfo(enemyType, out enemyInfo);
        enemyAI.Init(this.transform, enemyType, OnEdgeDetected);
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

    private void OnEdgeDetected()
    {
        if (!PatrolPaused)
        {
            pausePatrolUntil = Time.time + enemyInfo.patrolPause;
        }
    }
}