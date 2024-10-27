using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private IMovementBehavior movementBehavior;
    private IPursuitBehavior pursuitBehavior;
    private IAttackBehavior attackBehavior;

    public void Initialize(IMovementBehavior movement, IPursuitBehavior pursuit, IAttackBehavior attack)
    {
        movementBehavior = movement;
        pursuitBehavior = pursuit;
        attackBehavior = attack;
    }

    private void Update()
    {
        movementBehavior?.Execute();

        if (ShouldPursue())
        {
            pursuitBehavior?.Execute();
        }

        if (CanAttack())
        {
            attackBehavior?.Execute();
        }
    }

    private bool ShouldPursue()
    {
        // Implement pursuit condition (e.g., player is in detection range)
        return true;
    }

    private bool CanAttack()
    {
        // Implement attack condition (e.g., player is within attack range)
        return true;
    }
}
