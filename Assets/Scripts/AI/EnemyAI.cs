using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected IMovementBehavior movementBehavior;
    protected IPursuitBehavior pursuitBehavior;
    protected IAttackBehavior attackBehavior;

    public virtual void Init(Transform enemyTransform, EnemyType enemyType, System.Action onEdgeDetected)
    {
    }

    public void UpdateBehavior(bool isPursuingPlayer)
    {
        if (isPursuingPlayer)
        {
            pursuitBehavior?.Execute();
            attackBehavior?.Execute();
        }
        else
        {
            movementBehavior?.Execute();
        }
    }

    protected bool CanAttack()
    {
        return true;
    }
}