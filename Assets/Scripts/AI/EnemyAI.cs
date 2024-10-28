using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected IMovementBehavior movementBehavior;
    protected IPursuitBehavior pursuitBehavior;
    protected IAttackBehavior attackBehavior;

    public virtual void Init(Transform enemyTransform, float patrolSpeed, float pursuitSpeed, float attackRange, GameObject bulletPrefab, System.Action onEdgeDetected)
    {
        
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

    protected bool CanAttack()
    {
        // Implement attack condition (e.g., player is within attack range)
        return true;
    }
}