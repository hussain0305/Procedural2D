using UnityEngine;

public class ContactDetection_Ground : MonoBehaviour
{
    public PlayerController player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        player.isGrounded = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        player.isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player.isGrounded = false;
    }
}
