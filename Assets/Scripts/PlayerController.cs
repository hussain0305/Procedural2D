using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravityScale = 2f;
    public float wallSlideSpeed = 2f;

    [HideInInspector]
    public Rigidbody2D rb;
    
    private int additionalJumps = 0;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private int jumpsRemaining;
    private float groundCheckRadius = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        jumpsRemaining = additionalJumps;
        groundCheckRadius = (transform.localScale.x / 2) * 5 / 4;
    }

    void Update()
    {
        MovePlayer();
        CheckIfGrounded();
        CheckIfTouchingWall();
        HandleJump();
        HandleWallSlide();
    }

    void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpsRemaining > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    void HandleWallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, GlobalData.Instance.groundLayer);

        if (isGrounded)
        {
            jumpsRemaining = additionalJumps;
        }
    }

    void CheckIfTouchingWall()
    {
        isTouchingWall = Physics2D.OverlapCircle(transform.position, groundCheckRadius, GlobalData.Instance.wallLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check and wall check in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
    
    
    #region Ability
    public void IncrementNumJumps()
    {
        additionalJumps++;
    }
    #endregion
}
