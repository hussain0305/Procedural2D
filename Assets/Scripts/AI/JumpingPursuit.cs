using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPursuit : IPursuitBehavior
{
    private Transform enemy;
    private Transform target;
    private float jumpHeight;
    private float jumpDistance;

    public JumpingPursuit(Transform enemy, Transform target, float jumpHeight, float jumpDistance)
    {
        this.enemy = enemy;
        this.target = target;
        this.jumpHeight = jumpHeight;
        this.jumpDistance = jumpDistance;
    }

    public void Execute()
    {
        // Implement jumping pursuit logic
    }
}
