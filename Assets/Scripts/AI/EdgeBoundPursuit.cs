using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBoundPursuit : IPursuitBehavior
{
    private Transform enemy;
    private Transform target;
    private float speed;

    public EdgeBoundPursuit(Transform enemy, Transform target, float speed)
    {
        this.enemy = enemy;
        this.target = target;
        this.speed = speed;
    }

    public void Execute()
    {
        // Implement edge-bound pursuit logic
    }
}
