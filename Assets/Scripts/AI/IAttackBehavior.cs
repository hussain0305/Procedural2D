using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehavior
{
    public bool CanAttack { get; set; }
    public bool PlayerInView { get; set; }

    void Execute();
    bool CanSeePlayer();
}
