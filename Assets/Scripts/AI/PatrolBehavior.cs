using UnityEngine;
using System;

public class PatrolBehavior : IMovementBehavior
{
    public bool CanPatrol { get; set; }
    
    private Transform enemy;
    private float speed;
    private Vector2 patrolDirection;
    private Vector2 groundCheckDistance = new Vector2(0.5f, -0.5f);
    private Action onEdgeDetected;
    private LayerMask groundLayer;

    public PatrolBehavior(Transform enemy, float speed, Action onEdgeDetected)
    {
        CanPatrol = true;
        this.enemy = enemy;
        this.speed = speed;
        this.onEdgeDetected = onEdgeDetected;
        this.patrolDirection = Vector2.right;
        this.groundLayer = GlobalData.Instance.groundLayer;
    }

    public void Execute()
    {
        if (!CanPatrol) return;
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
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);

        Debug.DrawRay(origin, 0.1f * Vector2.down, Color.red);
        return hit.collider != null;
    }

    private void FaceDirection()
    {
        enemy.localScale = new Vector3(Mathf.Sign(patrolDirection.x) * Mathf.Abs(enemy.localScale.x), enemy.localScale.y, enemy.localScale.z);
    }
}