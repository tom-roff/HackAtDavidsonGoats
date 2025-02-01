using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float gravity = -50f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDeceleration = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float lastMoveDirection = 1f;
    private bool isDashing;
    private float dashTimeLeft;
    private float currentDashSpeed;
    private float dashCooldownTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleJump();
        HandleDash();
        if (!isDashing)
        {
            HandleMovement();
            ApplyGravity();
        }
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Store last move direction when moving
        if (horizontalInput != 0)
        {
            lastMoveDirection = Mathf.Sign(horizontalInput);
        }

        // Apply movement or dash speed
        float currentSpeed = isDashing ? currentDashSpeed : moveSpeed;
        Vector3 movement = new Vector3(horizontalInput * currentSpeed, 0, 0);
        controller.Move(movement * Time.deltaTime);
    }

    void HandleDash()
    {
        // Update dash cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Start dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0)
        {
            isDashing = true;
            dashTimeLeft = dashDuration;
            currentDashSpeed = dashSpeed;
            dashCooldownTimer = dashCooldown;
            velocity.y = 0;
        }

        // Handle ongoing dash
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            currentDashSpeed = Mathf.MoveTowards(currentDashSpeed, moveSpeed, dashDeceleration * Time.deltaTime);

            // Apply dash movement
            Vector3 dashMovement = new Vector3(lastMoveDirection * currentDashSpeed, 0, 0);
            controller.Move(dashMovement * Time.deltaTime);

            // End dash when time is up
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
