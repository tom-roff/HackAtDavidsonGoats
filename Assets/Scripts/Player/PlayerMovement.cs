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

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackResistance = 0f;

    // Public properties with PascalCase
    public bool IsDashing { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsFacingRight { get; private set; } = true;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private float lastMoveDirection = 1f;
    private float dashTimeLeft;
    private float currentDashSpeed;
    private float dashCooldownTimer;
    private int airDashesUsed;
    private Vector3 knockbackVelocity;
    private float knockbackTimeRemaining;
    private Transform playerModel;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = PlayerManager.Instance.PlayerAnimator;
        playerModel = PlayerManager.Instance.PlayerMesh.transform;
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
                
                // Update animator parameters
                anim.SetFloat("moveSpeed", Input.GetAxisRaw("Horizontal"));
                anim.SetBool("IsGrounded", IsGrounded);
            }
        }
        
        // Clamp player's Z-position to 0.
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        // Convert world direction to local direction
        Vector3 effectiveDirection = transform.InverseTransformDirection(direction);
        knockbackVelocity = new Vector3(effectiveDirection.x * force, 5f, 0);
        knockbackTimeRemaining = duration;
        velocity.y = 0;
    }

    private void HandleGroundCheck()
    {
        IsGrounded = controller.isGrounded;
        if (IsGrounded && velocity.y < 0)
        {
            anim.ResetTrigger("Jump");
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            if ((horizontalInput > 0 && !IsFacingRight) || (horizontalInput < 0 && IsFacingRight))
            {
                Flip();
            }
            lastMoveDirection = Mathf.Sign(horizontalInput);
        }

        float currentSpeed = IsDashing ? dashSpeed : moveSpeed;
        Vector3 movement = new Vector3(horizontalInput * currentSpeed, 0, 0);

        if (GameManager.Instance != null && GameManager.Instance.movementSpeedDouble)
        {
            controller.Move(movement * 2f * Time.deltaTime);
        }
        else
        {
            controller.Move(movement * Time.deltaTime);
        }
    }

    private void Flip()
    {
        playerModel.localRotation = Quaternion.Euler(playerModel.localRotation.eulerAngles.x,
                                                     playerModel.localRotation.eulerAngles.y + 180f,
                                                     playerModel.localRotation.eulerAngles.z);
        IsFacingRight = !IsFacingRight;
    }

    private void HandleDash()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (IsGrounded)
            airDashesUsed = 0;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !IsDashing && dashCooldownTimer <= 0)
        {
            if (!IsGrounded && airDashesUsed >= airDashLimit)
                return;

            IsDashing = true;
            anim.SetTrigger("Dash");
            dashTimeLeft = dashDuration;
            currentDashSpeed = dashSpeed;
            dashCooldownTimer = dashCooldown;
            velocity.y = 0;

            if (!IsGrounded)
                airDashesUsed++;
        }

        if (IsDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            currentDashSpeed = Mathf.MoveTowards(currentDashSpeed, moveSpeed, dashDeceleration * Time.deltaTime);
            Vector3 dashMovement = new Vector3(lastMoveDirection * currentDashSpeed, 0, 0);
            controller.Move(dashMovement * Time.deltaTime);

            if (dashTimeLeft <= 0)
            {
                anim.ResetTrigger("Dash");
                IsDashing = false;
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            anim.SetTrigger("Jump");
        }
    }

    private void ApplyGravity()
    {
        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        velocity.y = Mathf.Max(velocity.y, maxFallSpeed);
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleKnockback()
    {
        if (knockbackTimeRemaining > 0)
        {
            velocity.y += 15f * Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackResistance * Time.deltaTime);
            knockbackTimeRemaining -= Time.deltaTime;
        }
    }
}
