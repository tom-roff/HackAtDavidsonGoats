using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float gravity = -35f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = -25f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float dashDeceleration = 80f;
    [SerializeField] private int airDashLimit = 1;

    public bool IsDashing { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsFacingRight { get; private set; } = true;

    private CharacterController controller;
    private Vector3 velocity;
    private float lastMoveDirection = 1f;
    private float dashTimeLeft;
    private float currentDashSpeed;
    private float dashCooldownTimer;
    private int airDashesUsed;
    private float currentSpeed;
    private Vector3 knockbackVelocity;
    private float knockbackTimeRemaining;
    private float knockbackResistance;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleKnockback();
        HandleGroundCheck();
        
        if (knockbackTimeRemaining <= 0)
        {
            HandleDash();
            if (!IsDashing)
            {
                HandleMovement();
                HandleJump();
                ApplyGravity();
            }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        Vector3 effectiveDirection = transform.InverseTransformDirection(direction);
        knockbackVelocity = new Vector3(effectiveDirection.x * force, 5f, 0);
        knockbackTimeRemaining = duration;
        velocity.y = 0;
    }

void HandleZAxis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            playerAnimator.ResetTrigger("Jump");
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
            isFacingRight = horizontalInput > 0;
        }

        // Apply movement or dash speed
        currentSpeed = isDashing ? currentDashSpeed : moveSpeed;
        Vector3 movement = new Vector3(horizontalInput * currentSpeed, 0, 0);
        if(gameManager.movementSpeedDouble){
            controller.Move(movement * 2 * Time.deltaTime);
        }
        else{
            controller.Move(movement * Time.deltaTime);
        }

    }

    void HandleDash()
    {
        // Update dash cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Reset air dashes when grounded
        if (isGrounded)
        {
            airDashesUsed = 0;
        }

        // Start dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0)
        {
            // Check air dash availability
            if (!isGrounded && airDashesUsed >= airDashLimit)
            {
                return;
            }

            isDashing = true;
            playerAnimator.SetTrigger("Dash");
            dashTimeLeft = dashDuration;
            currentDashSpeed = dashSpeed;
            dashCooldownTimer = dashCooldown;
            velocity.y = 0;

            // Track air dash usage
            if (!isGrounded)
            {
                airDashesUsed++;
            }
        }

        // Moved inside HandleDash method
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
                playerAnimator.ResetTrigger("Dash");
                isDashing = false;
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            playerAnimator.SetTrigger("Jump");
        }
    }

    void ApplyGravity()
    {
        // Apply extra gravity when falling
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        // Clamp fall speed to prevent excessive velocity
        velocity.y = Mathf.Max(velocity.y, maxFallSpeed);
        
        controller.Move(velocity * Time.deltaTime);
    }


    void HandleKnockback()
    {
        if (knockbackTimeRemaining > 0)
        {
            // Apply vertical anti-stick force
            velocity.y += 15f * Time.deltaTime;
            
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackResistance * Time.deltaTime);
            knockbackTimeRemaining -= Time.deltaTime;
        }
    }
}
