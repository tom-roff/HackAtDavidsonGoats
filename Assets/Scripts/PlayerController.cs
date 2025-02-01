using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 2.5f;
    [SerializeField] private float gravity = -50f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float dashDeceleration = 80f;
    [SerializeField] private int airDashLimit = 1;
    private int airDashesUsed = 0;

    [Header("Combat Settings")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 1;
    [SerializeField] private float meleeDelay = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform meleePointRight;
    [SerializeField] private Transform meleePointLeft;
    [SerializeField] private Transform meleePointUp;
    [SerializeField] private Transform meleePointDown;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float lastMoveDirection = 1f;
    private bool isDashing;
    private float dashTimeLeft;
    private float currentDashSpeed;
    private float dashCooldownTimer;
    private float meleeTimer;
    private bool isFacingRight = true;
    private float currentSpeed;

    public GameObject playerHourglass;
    public Animator playerAnimator;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        meleeTimer = 0f;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleDash();
        if (!isDashing)
        {
            HandleJump();
            HandleMelee();
            HandleMovement();
            ApplyGravity();
        }
        if (meleeTimer > 0)
        {
            meleeTimer -= Time.deltaTime;
        }
        HandleZAxis();

        // Animation Work
        playerAnimator.SetFloat("moveSpeed", Input.GetAxisRaw("Horizontal"));
        Debug.Log(Input.GetAxisRaw("Horizontal"));
        playerAnimator.SetBool("IsGrounded", isGrounded);
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
        controller.Move(movement * Time.deltaTime);
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

    void HandleMelee()
    {
        if (Input.GetMouseButtonDown(0) && meleeTimer <= 0) // Left mouse button
        {
            Vector3 attackDirection;
            
            // Up attack (LMB + W)
            if (Input.GetKey(KeyCode.W))
            {
                attackDirection = Vector3.up;
            }
            // Down attack (LMB + S)
            else if (Input.GetKey(KeyCode.S))
            {
                attackDirection = Vector3.down;
            }
            // Regular horizontal attack
            else
            {
                attackDirection = isFacingRight ? Vector3.right : Vector3.left;
            }

            PerformMeleeAttack(attackDirection);
            meleeTimer = meleeDelay;
        }
    }


    void PerformMeleeAttack(Vector3 attackDirection)
    {
        Transform currentMeleePoint = meleePointRight; // Default to right

        // Determine melee point based on direction
        if (attackDirection == Vector3.up)
        {
            currentMeleePoint = meleePointUp;
        }
        else if (attackDirection == Vector3.down)
        {
            currentMeleePoint = meleePointDown;
        }
        else
        {
            currentMeleePoint = isFacingRight ? meleePointRight : meleePointLeft;
        }

        // Visual debug
        Debug.DrawRay(currentMeleePoint.position, attackDirection * meleeRange, Color.red, 0.1f);

        // Sphere cast from appropriate point
        RaycastHit[] hits = Physics.SphereCastAll(
            currentMeleePoint.position,
            0.5f,
            attackDirection,
            meleeRange,
            enemyLayer
        );

        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }
}
