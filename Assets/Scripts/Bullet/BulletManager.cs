using System.Collections.Generic;
using UnityEngine;

public static class BulletManager
{
    private static Dictionary<BulletType, Queue<Bullet>> bulletPools = new Dictionary<BulletType, Queue<Bullet>>();
    private static Dictionary<BulletType, Bullet> bulletPrefabs = new Dictionary<BulletType, Bullet>();

    public static void Sanitize()
    {
        if (bulletPools != null)
        {
            foreach (var pool in bulletPools)
            {
                var bulletQueue = pool.Value;

                while (bulletQueue.Count > 0)
                {
                    var bullet = bulletQueue.Dequeue();
                    if (bullet != null && bullet.gameObject != null)
                    {
                        GameObject.Destroy(bullet.gameObject);
                    }
                }
            }

            bulletPools.Clear();
        }
        
    }
    
    public static void InitializeBulletManager(BulletType type, Bullet prefab, int initialSize = 3)
    {
        if (!bulletPools.ContainsKey(type))
        {
            Queue<Bullet> pool = new Queue<Bullet>();
            for (int i = 0; i < initialSize; i++)
            {
                Bullet newBullet = GameObject.Instantiate(prefab);
                newBullet.gameObject.SetActive(false);
                pool.Enqueue(newBullet);
            }

            bulletPools[type] = pool;
            bulletPrefabs[type] = prefab;
        }
    }

    public static Bullet GetBullet(BulletType type, Vector3 position)
    {
        if (bulletPools.ContainsKey(type) && bulletPools[type].Count > 0)
        {
            Bullet bullet = bulletPools[type].Dequeue();
            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = position;
            return bullet;
        }
        else
        {
            // If no bullets are available, create a new one
            Bullet newBullet = GameObject.Instantiate(bulletPrefabs[type]);
            newBullet.gameObject.SetActive(true);
            newBullet.gameObject.transform.position = position;
            return newBullet;
        }
    }

    public static void ReturnBullet(BulletType type, Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletPools[type].Enqueue(bullet);
    }
}