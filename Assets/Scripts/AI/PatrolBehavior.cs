using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehavior : IMovementBehavior
{
    private Transform enemy;
    private Vector2 pointA;
    private Vector2 pointB;
    private float speed;

    public PatrolBehavior(Transform enemy, Vector2 pointA, Vector2 pointB, float speed)
    {
        this.enemy = enemy;
        this.pointA = pointA;
        this.pointB = pointB;
        this.speed = speed;
    }

    public void Execute()
    {
        // Implement patrolling logic between pointA and pointB
    }
}
