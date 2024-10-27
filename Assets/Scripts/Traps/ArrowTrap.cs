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

    private float arrowSpeed = 35;
    private float fireCooldown = 1;
    private float lastFireTime = 0;
    private float direction;
    private int damage;
    
    public override void Init(int _damage)
    {
        base.Init(_damage);
        direction = transform.localScale.x;
        BulletManager.InitializeBulletManager(bulletType, bullet);
        DetectGround();
        damage = _damage;
    }

    public void DetectGround()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position + new Vector3(direction,0,0), transform.right * direction, Global.CELL_SIZE_INTERIOR_X, GlobalData.Instance.groundLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position + new Vector3(direction,0,0), transform.right * direction, Global.CELL_SIZE_INTERIOR_X, GlobalData.Instance.wallLayer);

        float distanceToGround = Global.CELL_SIZE_INTERIOR_X;
        float distanceToWall = Global.CELL_SIZE_INTERIOR_X;

        if (groundHit.collider != null)
        {
            distanceToGround = Mathf.Abs(groundHit.point.x - transform.position.x);
        }

        if (wallHit.collider != null)
        {
            distanceToWall = Mathf.Abs(wallHit.point.x - transform.position.x);
        }

        float finalDistance = Mathf.Min(distanceToGround, distanceToWall);

        playerDetectionTrigger.size = new Vector2(finalDistance, playerDetectionTrigger.size.y);
        playerDetectionTrigger.offset = new Vector2(finalDistance / 2, playerDetectionTrigger.offset.y);    
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!simulate)
        {
            return;
        }
        if (Time.time > lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time;
            Bullet firedBullet = BulletManager.GetBullet(bulletType, lauchingPosition.position);
            firedBullet.damage = damage;
            firedBullet.Fire(firedBullet.transform.right * direction, arrowSpeed, new Vector3(direction, 1, 1));
        }
    }
}
