using UnityEngine;

public static class ProjectileMotion
{
    public static bool CalculateLaunchAngle(Vector2 startPos, Vector2 targetPos, float speed, out float angle, float gravityScale = 1)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * gravityScale);
        float distanceX = Mathf.Abs(targetPos.x - startPos.x);
        float distanceY = targetPos.y - startPos.y;

        float speedSquare = speed * speed;
        float insideSqrt = speedSquare * speedSquare - gravity * (gravity * distanceX * distanceX + 2 * distanceY * speedSquare);

        if (insideSqrt < 0)
        {
            angle = float.NaN;
            return false;
        }

        float sqrtPart = Mathf.Sqrt(insideSqrt);
        float lowAngleRad = Mathf.Atan((speedSquare - sqrtPart) / (gravity * distanceX));
        float highAngleRad = Mathf.Atan((speedSquare + sqrtPart) / (gravity * distanceX));

        if (IsPathClear(startPos, targetPos, speed, lowAngleRad * Mathf.Rad2Deg))
        {
            angle = lowAngleRad * Mathf.Rad2Deg;
            return true;
        }

        if (IsPathClear(startPos, targetPos, speed, highAngleRad * Mathf.Rad2Deg))
        {
            angle = highAngleRad * Mathf.Rad2Deg;
            return true;
        }

        angle = float.NaN;
        return false;
    }
    
    private static bool IsPathClear(Vector2 startPos, Vector2 targetPos, float speed, float angle)
    {
        Vector2[] trajectoryPoints = CalculateTrajectoryPoints(startPos, targetPos, speed, angle, 8);

        int layerMask = LayerMask.GetMask("Ground", "Wall", "Ceiling");

        for (int i = 0; i < trajectoryPoints.Length - 1; i++)
        {
            if (Physics2D.OverlapPoint(trajectoryPoints[i], layerMask))
            {
                DrawTrajectory(trajectoryPoints, Color.red);
                return false;
            }
        }

        DrawTrajectory(trajectoryPoints, Color.green);
        return true;
    }

    public static Vector2[] CalculateTrajectoryPoints(Vector2 startPoint, Vector2 targetPoint, float velocity, float angle, int numPointsPerSecond)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float initialVelocityX = velocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        float initialVelocityY = velocity * Mathf.Sin(angle * Mathf.Deg2Rad);

        float horizontalDistance = Mathf.Abs(targetPoint.x - startPoint.x);
        float totalTime = horizontalDistance / initialVelocityX;
        int numPoints = (int)(totalTime * numPointsPerSecond);
        Vector2[] trajectoryPoints = new Vector2[numPoints];
        float timeStep = totalTime / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;
            float x = startPoint.x + initialVelocityX * t * Mathf.Sign(targetPoint.x - startPoint.x);
            float y = startPoint.y + initialVelocityY * t - 0.5f * gravity * t * t;

            trajectoryPoints[i] = new Vector2(x, y);
        }

        return trajectoryPoints;
    }
    
    private static void DrawTrajectory(Vector2[] points, Color color)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], color, 1f);
        }
    }
}