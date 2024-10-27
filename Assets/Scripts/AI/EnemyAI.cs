using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private IMovementBehavior movementBehavior;
    private IPursuitBehavior pursuitBehavior;
    private IAttackBehavior attackBehavior;

    public void Init(Transform enemyTransform, float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab, System.Action onEdgeDetected)
    {
        movementBehavior = new PatrolBehavior(enemyTransform, patrolSpeed, onEdgeDetected);
        pursuitBehavior = new EdgeBoundPursuit(enemyTransform, GameManager.Instance.player.transform, pursuitSpeed);
        attackBehavior = new RangedAttack(enemyTransform, GameManager.Instance.player.transform, attackRange, bulletPrefab);
    }

    public void UpdateBehavior(bool isPursuingPlayer)
    {
        if (isPursuingPlayer)
        {
            pursuitBehavior?.Execute();
            if (CanAttack())
            {
                attackBehavior?.Execute();
            }
        }
        else
        {
            movementBehavior?.Execute();
        }
    }

    private bool CanAttack()
    {
        // Implement attack condition (e.g., player is within attack range)
        return true;
    }
}