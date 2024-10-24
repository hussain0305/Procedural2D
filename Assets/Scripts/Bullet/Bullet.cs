using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);
        BulletManager.ReturnBullet(bulletType, this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with " + other.gameObject.name);
        BulletManager.ReturnBullet(bulletType, this);
    }

    public void Fire(Vector2 direction, float speed, Vector3 facingDirection)
    {
        transform.localScale = facingDirection;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
}
