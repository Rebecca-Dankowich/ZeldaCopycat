using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Jump Settings")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    public Transform orientation;

    //Private variables
    private bool grounded;
    private bool readyToJump = true;
    Vector3 moveDirection;
    Rigidbody rb;
    private PlayerInputHandler input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        input = GetComponent<PlayerInputHandler>();

    }
    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), grounded ? Color.green : Color.red);

        SpeedControl();

        // handle drag
        rb.linearDamping = grounded ? groundDrag : 0f;

        if (input.JumpTriggered && readyToJump && grounded)
        {
            readyToJump = false;
            HandleJump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void MovePlayer()
    {
        // Map 2D input (x = horizontal, y = vertical) to 3D world direction
        moveDirection = orientation.forward * input.MoveInput.y + orientation.right * input.MoveInput.x;

        // On ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            // Check if paraglider is active
            Paraglider paraglider = GetComponent<Paraglider>();
            bool gliding = paraglider != null && paraglider.IsGliding();

            float multiplier = gliding ? 0f : airMultiplier;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * multiplier, ForceMode.Force);
        }
    }
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // clamp the velocity
        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void HandleJump()
    {
        // reset y velocity to ensure consistent jump height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}

