using System.Collections;
using UnityEngine;

public class PatrolBehavior : IMovementBehavior
{
    private Transform enemy;
    private Vector2 pointA;
    private Vector2 pointB;
    private float speed;
    private float pauseDuration;
    private bool isPaused;

    private Vector2 targetPoint;

    public PatrolBehavior(Transform enemy, Vector2 pointA, Vector2 pointB, float speed, float pauseDuration)
    {
        this.enemy = enemy;
        this.pointA = pointA;
        this.pointB = pointB;
        this.speed = speed;
        this.pauseDuration = pauseDuration;
        this.targetPoint = pointA;
    }

    public void Execute()
    {
        if (!isPaused)
        {
            float step = speed * Time.deltaTime;
            enemy.position = Vector2.MoveTowards(enemy.position, targetPoint, step);

            if (Vector2.Distance(enemy.position, targetPoint) < 0.1f)
            {
                targetPoint = targetPoint == pointA ? pointB : pointA;
                enemy.GetComponent<MonoBehaviour>().StartCoroutine(PauseAtPoint());
            }
        }
    }

    private IEnumerator PauseAtPoint()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}