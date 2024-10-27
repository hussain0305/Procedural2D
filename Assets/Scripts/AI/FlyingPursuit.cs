using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPursuit : IPursuitBehavior
{
    private Transform enemy;
    private Transform target;
    private float speed;

    public FlyingPursuit(Transform enemy, Transform target, float speed)
    {
        this.enemy = enemy;
        this.target = target;
        this.speed = speed;
    }

    public void Execute()
    {
        // Implement flying pursuit logic
    }
}
