using UnityEngine;

public class Paraglider : MonoBehaviour
{
    [Header("Paraglider Settings")]
    public float fallSpeed = -2f;
    public float glideAirMultiplier = 2f;
    public float deploymentDelay = 0.2f;
    public KeyCode paragliderKey = KeyCode.Space;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private bool isGliding = false;
    private bool grounded;
    private float deployTimer = 0f;

    public float playerHeight;
    public LayerMask whatIsGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
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
            isGliding = false; // cancel glide on landing
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
        // to deploy player must be airborne and past the deployment delay
        if (Input.GetKeyDown(paragliderKey) && !grounded && deployTimer > deploymentDelay)
        {
            isGliding = !isGliding; // toggle glider
        }

        if (grounded)
        {
            isGliding = false; // force cancel if player lands
        }
    }

    private void ApplyGlide()
    {
        // clamp the downward velocity to simulate a slow fall
        if(rb.linearVelocity.y < fallSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fallSpeed, rb.linearVelocity.z);
        }

        rb.AddForce(Vector3.up * Physics.gravity.magnitude * 0.85f, ForceMode.Acceleration);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 glideDirection = orientation.forward * v + orientation.right * h;
        rb.AddForce(glideDirection * glideAirMultiplier, ForceMode.Force);
    }
     // public reference for access to be used in other scripts
    public bool IsGliding() => isGliding;
}
