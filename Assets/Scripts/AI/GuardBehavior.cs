using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehavior : IMovementBehavior
{
    public bool CanPatrol { get; set; }

    private Transform enemy;

    public GuardBehavior(Transform enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        // Implement guarding logic
    }
}
