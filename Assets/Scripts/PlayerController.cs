using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravityScale = 2f;
    public Weapon equippedWeapon;

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
    private Vector2 lastDirection = Vector2.right;
    
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
        CheckFireRay();
    }

    void MovePlayer()
    {
        float moveInputHorizontal = Input.GetAxis("Horizontal");
        float moveInputVertical = Input.GetAxis("Vertical");
        if (moveInputVertical != 0)
        {
            lastDirection = new Vector2(0, Mathf.Sign(moveInputVertical));
        }
        else if (moveInputHorizontal != 0)
        {
            lastDirection = new Vector2(Mathf.Sign(moveInputHorizontal), 0);
        }
        
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void ForceStopPlayer()
    {
        rb.velocity = Vector2.zero;
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
    
    void CheckFireRay()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            equippedWeapon.PerformPrimaryAttack(lastDirection);
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
