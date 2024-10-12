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
    [HideInInspector] 
    public bool isGrounded = true;
    [HideInInspector] 
    public bool isTouchingWall = false;
    [HideInInspector] 
    public Abilities abilities;

    private int jumpsRemaining;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        jumpsRemaining = abilities.additionalJumps;
    }

    void Update()
    {
        MovePlayer();
        UpdatesNumJumps();
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
        if (abilities.hasWallGrab && Input.GetButton("WallGrab") && isTouchingWall)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        else// if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale;
        }
    }

    void UpdatesNumJumps()
    {
        if (isGrounded)
        {
            jumpsRemaining = abilities.additionalJumps;
        }
    }
    
    #region Ability
    public void IncrementNumJumps()
    {
        abilities.additionalJumps++;
    }
    #endregion
}
