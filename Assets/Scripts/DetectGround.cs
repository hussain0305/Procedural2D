using System;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    public float raycastDistance = 10f;

    private void Start()
    {
        PlaceOnGround();
    }

    public void PlaceOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, GlobalData.Instance.groundLayer);

        if (hit.collider != null)
        {
            Vector3 newPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("No ground or wall detected below.");
        }
    }
}