using System.Collections;
using UnityEngine;

public class ProjectileTester : MonoBehaviour
{
    public Transform bulletPrefab;
    public float speed = 15f;
    public bool useHighAngle = true;
    
    private Rigidbody2D bulletRb;

    private void Start()
    {
        bulletRb = bulletPrefab.GetComponent<Rigidbody2D>();
        StartCoroutine(TestProjectile());
    }

    private IEnumerator TestProjectile()
    {
        while (true)
        {
            Vector2 startPos = transform.position;
            Vector2 targetPos = GameManager.Instance.player.transform.position;

            bool isTargetLeft = targetPos.x < startPos.x;
            float angle = ProjectileMotion.CalculateLaunchAngle(startPos, targetPos, speed, useHighAngle);
            if (!float.IsNaN(angle))
            {
                yield return new WaitForSeconds(1);
                CalculateAndShoot(angle, isTargetLeft);

                Vector2[] trajectoryPoints = ProjectileMotion.CalculateTrajectoryPoints(startPos, speed, angle, 10, isTargetLeft);
                DrawTrajectory(trajectoryPoints);
            }
            else
            {
                Debug.LogWarning("Target is out of range.");
            }
            yield return null;
        }
    }

    private void CalculateAndShoot(float angle, bool isTargetLeft)
    {
        float radAngle = angle * Mathf.Deg2Rad;
        Vector2 launchVelocity = new Vector2(speed * Mathf.Cos(radAngle) * (isTargetLeft ? -1 : 1), speed * Mathf.Sin(radAngle));
        bulletRb.velocity = launchVelocity;
    }

    private void DrawTrajectory(Vector2[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red, 1f);
        }
    }
}