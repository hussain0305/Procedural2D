using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;

    void OnCollisionEnter2D(Collision2D collision)
    {
        BulletManager.ReturnBullet(bulletType, this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BulletManager.ReturnBullet(bulletType, this);
    }

    public void Fire(Vector2 direction, float speed)
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
}
