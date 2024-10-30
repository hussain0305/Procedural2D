using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public IMovementBehavior movementBehavior;
    public IPursuitBehavior pursuitBehavior;
    public IAttackBehavior attackBehavior;

    public virtual void Init(Transform enemyTransform, EnemyType enemyType, System.Action onEdgeDetected, System.Action onPlayerDetected, System.Action onPlayerLost)
    {
    }

    public void UpdateBehavior(bool isPursuingPlayer)
    {
        movementBehavior?.Execute();
        attackBehavior?.Execute();
    }

    protected bool CanAttack()
    {
        return true;
    }
}