using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
    public PFXDatabase.PFXType hitEffect;
    public Transform head;
    
    [HideInInspector]
    public int damage;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform && collision.transform.CompareTag("Player"))
        {
            DamagePlayer(collision.transform.GetComponent<PlayerAttributes>());
        }

        if (collision.contactCount > 0)
        {
            ContactPoint2D point = collision.GetContact(0);
            BulletHitSomething(point.point);
        }
        else
        {
            BulletHitSomething();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform && other.transform.CompareTag("Player"))
        {
            DamagePlayer(other.transform.GetComponent<PlayerAttributes>());
        }
        
        BulletHitSomething();
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

    public void BulletHitSomething()
    {
        PFXManager.Instance.PlayPFX(hitEffect, head ? head.position : transform.position);
        BulletManager.ReturnBullet(bulletType, this);
    }
    
    public void BulletHitSomething(Vector3 position)
    {
        PFXManager.Instance.PlayPFX(hitEffect, position);
        BulletManager.ReturnBullet(bulletType, this);
    }
}
