using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour //This class name must match that in the asset, no automatic refactoring
{

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 14f;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float thresholdVelocity = 3f;
    [SerializeField] private float thresholdDragFactor = 4f;
    private bool facingLeft = true;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer = 0f;

    [Header("Collision")]
    public bool isGrounded = false; // Public because GroundCheck needs to deal with this
    [SerializeField] private float distanceToGround = 0.01f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;

    [Header("Physics")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float linearDrag = 5f;
    [SerializeField] private float gravity = 1.4f;
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float linearDragJumpScale = 0.15f;

    // Update is called once per frame
    void Update() {
        // Raycast to floor to see if we are grounded, only needs to be called once per frame
        // TODO: change this to a boxcast
        //isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround);

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            jumpTimer = Time.time + jumpDelay;
        }

        HandleAnimation(direction.x);
    }

    // FixedUpdate is better for physics because can be called
    // multiple times per frame
    void FixedUpdate() {
        Move(direction.x);
        ApplyGravity();
        ApplyDrag();

        // delay time to allow for multiple frames of jumpp
        if (jumpTimer > Time.time && isGrounded) {
            Jump();
        }
    }

    void Move(float horizontal) {
        // actual movement
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        // max vel
        if (Mathf.Abs(rb.velocity.x) > maxSpeed) {
            rb.velocity = new Vector2(Math.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    // Drag is used for slowing down after letting go of the run button
    void ApplyDrag() {
        bool isChangingDir = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);
        if (isGrounded) {
            if (Mathf.Abs(direction.x) < 0.4f || isChangingDir) {
                if (Math.Abs(rb.velocity.x) < thresholdVelocity) {
                    rb.drag = linearDrag * thresholdDragFactor;
                } else {
                    rb.drag = linearDrag;
                }
            } else {
                rb.drag = 0;
            }
        }
    }

    void ApplyGravity() {
        // See "Better Jumping in Unity with Four Lines of Code"
        // For most of the garbage
        // https://www.youtube.com/watch?v=7KiK0Aqtmzc 
        if (!isGrounded) {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * linearDragJumpScale;

            if (rb.velocity.y < 0) {
                rb.gravityScale = gravity * fallMultiplier;
            } else if (rb.velocity.y > 0 && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) {
                rb.gravityScale = gravity * fallMultiplier * 0.5f;
            }

        } else {
            rb.gravityScale = 0;
        }
    }

    // Helper for changing the direction the player is lookin
    void Flip() {
        facingLeft = !facingLeft;
        transform.rotation = Quaternion.Euler(0, facingLeft ? 0 : 180, 0);
    }

    void Jump() {
        // reset velocity then jump
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void HandleAnimation(float horizontal) {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("Horizontal", Math.Abs(direction.x));
        if (horizontal < 0 && !facingLeft || horizontal > 0 && facingLeft) {
            Flip();
        }            
    }
}
