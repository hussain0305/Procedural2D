using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
    
    [HideInInspector]
    public int damage;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform && collision.transform.CompareTag("Player"))
        {
            DamagePlayer(collision.transform.GetComponent<PlayerAttributes>());
        }
        BulletManager.ReturnBullet(bulletType, this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform && other.transform.CompareTag("Player"))
        {
            DamagePlayer(other.transform.GetComponent<PlayerAttributes>());
        }
        BulletManager.ReturnBullet(bulletType, this);
    }

    public void Fire(Vector2 direction, float speed, Vector3 facingDirection)
    {
        transform.localScale = facingDirection;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    public void DamagePlayer(PlayerAttributes playerAttributes)
    {
        playerAttributes?.TakeDamage(damage);
    }
}
