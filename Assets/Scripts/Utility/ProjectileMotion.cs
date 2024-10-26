using UnityEngine;

public static class ProjectileMotion
{
    public static float CalculateLaunchAngle(Vector2 startPos, Vector2 targetPos, float speed, float gravityScale = 1, bool highAngle = true)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * gravityScale);
        float distanceX = Mathf.Abs(targetPos.x - startPos.x);
        float distanceY = targetPos.y - startPos.y;

        float speedSquare = speed * speed;
        float insideSqrt = speedSquare * speedSquare - gravity * (gravity * distanceX * distanceX + 2 * distanceY * speedSquare);

        if (insideSqrt < 0)
        {
            return float.NaN;
        }

        float sqrtPart = Mathf.Sqrt(insideSqrt);
        float angleRad = Mathf.Atan((speedSquare + (highAngle ? sqrtPart : -sqrtPart)) / (gravity * distanceX));

        return angleRad * Mathf.Rad2Deg;
    }

    public static Vector2[] CalculateTrajectoryPoints(Vector2 startPoint, Vector2 targetPoint, float velocity, int numPoints)
    {
        Vector2[] trajectoryPoints = new Vector2[numPoints];
        Vector2 direction = (targetPoint - startPoint).normalized;

        float totalDistance = Vector2.Distance(startPoint, targetPoint);
        float totalTime = totalDistance / velocity;
        float timeStep = totalTime / (numPoints - 1);

        float gravity = Mathf.Abs(Physics2D.gravity.y);

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = startPoint.x + velocity * t * direction.x;
            float y = startPoint.y + velocity * t * direction.y - 0.5f * gravity * t * t;
            trajectoryPoints[i] = new Vector2(x, y);
        }

        return trajectoryPoints;
    }
}