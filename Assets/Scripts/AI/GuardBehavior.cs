using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehavior : IMovementBehavior
{
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
