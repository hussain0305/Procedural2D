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
    private bool canGrab = true;
    private bool isGrabbingWall = false;
    
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
        HandleWallGrab();
        HandleJump();
        Debug.Log("Jumps: " + jumpsRemaining + "| is grounded = " + isGrounded);
    }

    void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        IEnumerator ResetCanGrab()
        {
            yield return new WaitForSeconds(0.2f);
            canGrab = true;
        }

        void PerformJump()
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                jumpsRemaining = abilities.additionalJumps;
                PerformJump();
            }
            else if (isGrabbingWall)
            {
                canGrab = false;
                isGrabbingWall = false;
                rb.gravityScale = gravityScale;
                jumpsRemaining = abilities.additionalJumps;
                PerformJump();
                StartCoroutine(ResetCanGrab());
            }
            else if(jumpsRemaining > 0)
            {
                jumpsRemaining--;
                PerformJump();
            }
        }
    }

    void HandleWallGrab()
    {
        if (Input.GetButton("WallGrab") && isTouchingWall && abilities.hasWallGrab && canGrab)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            isGrabbingWall = true;
        }
        else
        {
            isGrabbingWall = false;
            rb.gravityScale = gravityScale;
        }
    }

    void UpdatesNumJumps()
    {
        if (isGrounded)
        {
            Debug.Log("Reset here");
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
