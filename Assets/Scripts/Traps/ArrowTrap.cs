using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : Trap
{
    public Transform lauchingPosition;
    public BoxCollider2D playerDetectionTrigger;
    public BulletType bulletType;
    public Bullet bullet;
    
    public override void Init()
    {
        BulletManager.InitializeBulletManager(bulletType, bullet);
        DetectGround();
    }

    public void DetectGround()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position + new Vector3(1,0,0), transform.right, Global.CELL_SIZE_INTERIOR_X, GlobalData.Instance.groundLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position + new Vector3(1,0,0), transform.right, Global.CELL_SIZE_INTERIOR_X, GlobalData.Instance.wallLayer);

        float distanceToGround = Global.CELL_SIZE_INTERIOR_X;
        float distanceToWall = Global.CELL_SIZE_INTERIOR_X;

        if (groundHit.collider != null)
        {
            distanceToGround = groundHit.point.x - transform.position.x;
        }

        if (wallHit.collider != null)
        {
            distanceToWall = wallHit.point.x - transform.position.x;
        }

        float finalDistance = Mathf.Min(distanceToGround, distanceToWall);

        playerDetectionTrigger.size = new Vector2(finalDistance, playerDetectionTrigger.size.y);
        playerDetectionTrigger.offset = new Vector2(finalDistance / 2, playerDetectionTrigger.offset.y);    
    }
}
