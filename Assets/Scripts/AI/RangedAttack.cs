using UnityEngine;

public class RangedAttack : IAttackBehavior
{
    private Transform enemy;
    private Transform player;
    private float attackRange;
    private GameObject bulletPrefab;
    private float viewDistance;
    private float viewAngle;

    public RangedAttack(Transform enemy, Transform player, float attackRange, GameObject bulletPrefab, float viewDistance, float viewAngle)
    {
        this.enemy = enemy;
        this.player = player;
        this.attackRange = attackRange;
        this.bulletPrefab = bulletPrefab;
        this.viewDistance = viewDistance;
        this.viewAngle = viewAngle;
    }

    public bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - enemy.position).normalized;
        float distanceToPlayer = Vector2.Distance(enemy.position, player.position);

        if (distanceToPlayer > viewDistance) return false;

        float angleToPlayer = Vector2.Angle(enemy.right * Mathf.Sign(enemy.localScale.x), directionToPlayer);
        return angleToPlayer < viewAngle / 2;
    }

    public void Execute()
    {
        if (CanSeePlayer() && Vector2.Distance(enemy.position, player.position) <= attackRange)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, enemy.position, Quaternion.identity);
        // Set bullet velocity or direction towards the player here
    }
}