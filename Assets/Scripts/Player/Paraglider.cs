using UnityEngine;
using UnityEngine.Events;

public class Paraglider : MonoBehaviour
{
    [Header("Paraglider Settings")]
    public float fallSpeed = -2f;
    public float glideAirMultiplier = 2f;
    public float deploymentDelay = 0.2f;

    [Header("References")]
    public Transform orientation;

    [Header("Events")]
    public UnityEvent onGlideStart;
    public UnityEvent onGlideStop;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerInputHandler input;

    private bool isGliding = false;
    private bool grounded;
    private float deployTimer = 0f;
    private bool wasJumpHeld = false;

    public float playerHeight;
    public LayerMask whatIsGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // Sync the grounded state with playerMovement
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 2.0f, whatIsGround);

        //Count up deploy timer to prevent deploying accidentally mid jump
        if(!grounded)
        {
            deployTimer += Time.deltaTime;
        }
        else
        {
            deployTimer = 0f;
            SetGliding(false);
            wasJumpHeld = false;
        }
        HandleParagliderInput();
    }

    private void FixedUpdate()
    {
        if(isGliding)
        {
            ApplyGlide();
        }
    }
    private void HandleParagliderInput()
    {
        if (grounded) return;

        bool jumpHeld = input.JumpHeld;

        bool freshPress = jumpHeld && !wasJumpHeld;     // Detect a fresh press while airborne (wasJumpHeld prevents re-toggling on hold)

        if(freshPress && deployTimer > deploymentDelay)
        {
            SetGliding(!isGliding);
        }

        // If player releases the button, cancel the glide
        if (!jumpHeld)
        {
            SetGliding(false);
        }

        wasJumpHeld = jumpHeld;
    }

    private void SetGliding(bool value)
    {
        if (isGliding == value) return;

        isGliding = value;

        if (isGliding)
            onGlideStart.Invoke();
        else
            onGlideStop.Invoke();
    }

    private void ApplyGlide()
    {
        // clamp the downward velocity to simulate a slow fall
        if(rb.linearVelocity.y < fallSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fallSpeed, rb.linearVelocity.z);
        }

        rb.AddForce(Vector3.up * Physics.gravity.magnitude * 0.85f, ForceMode.Acceleration);

        Vector3 glideDirection = orientation.forward * input.MoveInput.y + orientation.right * input.MoveInput.x;
        rb.AddForce(glideDirection * glideAirMultiplier, ForceMode.Force);
    }
     // public reference for access to be used in other scripts
    public bool IsGliding() => isGliding;
}
