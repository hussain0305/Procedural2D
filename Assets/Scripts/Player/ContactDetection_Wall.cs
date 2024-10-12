using UnityEngine;

public class ContactDetection_Wall : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.isTouchingWall = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        player.isTouchingWall = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player.isTouchingWall = false;
    }
}
