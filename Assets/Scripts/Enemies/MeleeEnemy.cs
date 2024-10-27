using UnityEngine;

public class MeleeEnemy : Enemy
{
    public override void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, Player.position) <= enemyInfo.attackRange)
        {
            // Perform melee attack
            Debug.Log("Melee Attack");
        }
    }
}