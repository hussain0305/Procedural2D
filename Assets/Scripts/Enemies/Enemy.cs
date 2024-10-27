using System;
using System.Collections;
using UnityEngine;

public class Enemy : Damageable
{
    [HideInInspector] public Vector2Int locatedInRoom;
    [HideInInspector] public bool simulate;

    public EnemyAI enemyAI;
    protected EnemyData.EnemyInfo enemyInfo;
    protected bool isPursuingPlayer;
    [HideInInspector] public int damage;

    protected Transform Player => GameManager.Instance.player.transform;
    private bool isPaused;
    private float pauseDuration = 1.0f;

    public void Init(float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab, EnemyData.EnemyInfo _enemyInfo)
    {
        EventManager.OnRoomEntered += HandleRoomEntered;

        enemyInfo = _enemyInfo;
        enemyAI.Init(this.transform, patrolSpeed, pursuitSpeed, attackRange, bulletPrefab, OnEdgeDetected);
    }

    private void HandleRoomEntered(Vector2Int gridIndex)
    {
        simulate = GameManager.Instance.ShouldSimulate(locatedInRoom, gridIndex);
    }

    private void Update()
    {
        if (!simulate)
        {
            return;
        }
        
        if (!isPaused)
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
        if (!isPaused)
        {
            StartCoroutine(PauseAtEdge());
        }
    }

    private IEnumerator PauseAtEdge()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }

    private void OnDestroy()
    {
        EventManager.OnRoomEntered -= HandleRoomEntered;
    }
}