using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravityScale = 2f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private int jumpsRemaining;
    private Vector2 velocity;
    private int numJumps = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        jumpsRemaining = numJumps;
    }

    void Update()
    {
        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        rb.velocity = velocity;

        // Jumping logic
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            Jump();
        }
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log(" JUMP PRESSED " + jumpsRemaining);
        }
    }

    void Jump()
    {
        Debug.Log(" JUMPING");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpsRemaining--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpsRemaining = numJumps;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}