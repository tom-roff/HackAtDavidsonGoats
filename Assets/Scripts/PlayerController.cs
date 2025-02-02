using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
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
    private int airDashesUsed = 0;

    [Header("Combat Settings")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 1;
    [SerializeField] private float meleeDelay = 0.5f;
    [SerializeField] private float knockbackResistance = 3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform meleePointRight;
    [SerializeField] private Transform meleePointLeft;
    [SerializeField] private Transform meleePointUp;
    [SerializeField] private Transform meleePointDown;
    private Vector3 knockbackVelocity;
    private float knockbackTimeRemaining;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float lastMoveDirection = 1f;
    public bool isDashing;
    private float dashTimeLeft;
    private float currentDashSpeed;
    private float dashCooldownTimer;
    private float meleeTimer;
    private bool isFacingRight = true;
    private float currentSpeed;

    public GameObject playerHourglass;
    public Animator playerAnimator;
    public GameObject playerMesh;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        meleeTimer = 0f;
    }

    void Update()
    {
        HandleKnockback();
        
        // Only process inputs if not in knockback
        if (knockbackTimeRemaining <= 0)
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
        }
        
        HandleZAxis();

        // Animation Work
        playerAnimator.SetFloat("moveSpeed", Input.GetAxisRaw("Horizontal"));
        playerAnimator.SetBool("IsGrounded", isGrounded);

        if(isFacingRight == true){
            playerMesh.transform.rotation = Quaternion.Euler(playerMesh.transform.eulerAngles.x, 90, playerMesh.transform.eulerAngles.z);
        }
        else{
            playerMesh.transform.rotation = Quaternion.Euler(playerMesh.transform.eulerAngles.x, 270, playerMesh.transform.eulerAngles.z);
        }
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

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        // Convert direction to world space if enemy is rotated
        Vector3 effectiveDirection = transform.InverseTransformDirection(direction);
        
        // Apply knockback with consistent upward force
        knockbackVelocity = new Vector3(effectiveDirection.x * force, 5f, 0);
        knockbackTimeRemaining = duration;
        
        // Reset vertical velocity and disable gravity during knockback
        velocity.y = 0;
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
