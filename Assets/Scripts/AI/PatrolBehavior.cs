using UnityEngine;
using System;

public class PatrolBehavior : IMovementBehavior
{
    private Transform enemy;
    private float speed;
    private Vector2 patrolDirection;
    private Vector2 groundCheckDistance = new Vector2(0.5f, -0.5f);
    private Action onEdgeDetected;
    private LayerMask groundLayer;

    public PatrolBehavior(Transform enemy, float speed, Action onEdgeDetected)
    {
        this.enemy = enemy;
        this.speed = speed;
        this.onEdgeDetected = onEdgeDetected;
        this.patrolDirection = Vector2.right;
        this.groundLayer = GlobalData.Instance.groundLayer;
    }

    public void Execute()
    {
        if (onEdgeDetected == null) return;
        
        enemy.Translate(patrolDirection * (speed * Time.deltaTime));
        FaceDirection();

        if (!GroundAhead())
        {
            patrolDirection = -patrolDirection;
            onEdgeDetected.Invoke();
        }
    }

    private bool GroundAhead()
    {
        Vector2 origin = (Vector2)enemy.position + new Vector2(patrolDirection.x * groundCheckDistance.x, groundCheckDistance.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Mathf.Abs(groundCheckDistance.y), groundLayer);

        Debug.DrawRay(origin, Vector2.down * Mathf.Abs(groundCheckDistance.y), Color.red);
        return hit.collider != null;
    }

    private void FaceDirection()
    {
        enemy.localScale = new Vector3(Mathf.Sign(patrolDirection.x) * Mathf.Abs(enemy.localScale.x), enemy.localScale.y, enemy.localScale.z);
    }
}