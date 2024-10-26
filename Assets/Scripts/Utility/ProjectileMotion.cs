using UnityEngine;

public static class ProjectileMotion
{
    public static float CalculateLaunchAngle(Vector2 startPos, Vector2 targetPos, float speed, bool highAngle = true)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float distanceX = Mathf.Abs(targetPos.x - startPos.x);
        float distanceY = targetPos.y - startPos.y;

        float speedSquare = speed * speed;
        float insideSqrt = speedSquare * speedSquare - gravity * (gravity * distanceX * distanceX + 2 * distanceY * speedSquare);

        if (insideSqrt < 0)
        {
            Debug.LogWarning("Target out of range.");
            return float.NaN;
        }

        float sqrtPart = Mathf.Sqrt(insideSqrt);
        float angleRad = Mathf.Atan((speedSquare + (highAngle ? sqrtPart : -sqrtPart)) / (gravity * distanceX));

        return angleRad * Mathf.Rad2Deg;
    }

    public static Vector2[] CalculateTrajectoryPoints(Vector2 startPos, float speed, float angle, int numPoints, bool isTargetLeft)
    {
        Vector2[] trajectoryPoints = new Vector2[numPoints];
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float radAngle = angle * Mathf.Deg2Rad;

        float vx = speed * Mathf.Cos(radAngle) * (isTargetLeft ? -1 : 1);
        float vy = speed * Mathf.Sin(radAngle);

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * 0.1f;
            float x = startPos.x + vx * t;
            float y = startPos.y + vy * t - 0.5f * gravity * t * t;
            trajectoryPoints[i] = new Vector2(x, y);
        }
        return trajectoryPoints;
    }
}