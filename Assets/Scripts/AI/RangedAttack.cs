using UnityEngine;

public class RangedAttack : IAttackBehavior
{
    public bool CanAttack
    {
        get
        {
            return PlayerInView && Time.time > nextAttackTime;
        }
        set {}
    }

    public bool PlayerInView { get; set; }
    
    private Transform enemy;
    private Transform player;
    private float attackRange;
    private float attackCooldown;
    private GameObject bulletPrefab;
    private float viewDistance;
    private float viewAngle;
    private System.Action onPlayerDetected;
    private System.Action onPlayerLost;

    private float nextAttackTime = 0;

    public RangedAttack(Transform enemy, Transform player, float attackRange, float attackCooldown, GameObject bulletPrefab, float viewDistance, float viewAngle, System.Action onPlayerDetected, System.Action onPlayerLost)
    {
        this.enemy = enemy;
        this.player = player;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
        this.bulletPrefab = bulletPrefab;
        this.viewDistance = viewDistance;
        this.viewAngle = viewAngle;
        this.onPlayerDetected = onPlayerDetected;
        this.onPlayerLost = onPlayerLost;
    }

    public bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - enemy.position).normalized;
        float distanceToPlayerX = player.position.x - enemy.position.x;
        float distanceToPlayerY = player.position.y - enemy.position.y;

        if (distanceToPlayerX < viewDistance && distanceToPlayerY < viewDistance)
        {
            float angleToPlayer = Vector2.Angle(enemy.right * Mathf.Sign(enemy.localScale.x), directionToPlayer);
            return angleToPlayer < viewAngle / 2;
        }

        return false;
    }

    public void Execute()
    {
        bool lastPlayerInVIew = PlayerInView;
        PlayerInView = CanSeePlayer();
        if (lastPlayerInVIew != PlayerInView)
        {
            if (PlayerInView) onPlayerDetected?.Invoke();
            else onPlayerLost?.Invoke();
        }

        if (CanAttack)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        nextAttackTime += attackCooldown;
    }
}