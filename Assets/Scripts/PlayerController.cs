using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float knockbackResistance = 0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform meleePointRight;
    [SerializeField] private Transform meleePointLeft;
    [SerializeField] private Transform meleePointUp;
    [SerializeField] private Transform meleePointDown;
    [Header("Bone Settings")]
    [SerializeField] private GameObject boneProjectilePrefab;
    [SerializeField] private GameObject heldBone; // The visible bone in hand
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float boneCooldown = 1f;
    private bool hasBone = true;
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

    protected GameManager gameManager;

    public static PlayerController Instance { get; private set; }

        void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        if(SceneManager.GetActiveScene().name == "Level1"){
            playerHourglass.SetActive(true);

        }

        gameObject.transform.position = GameObject.Find("InitialSpawn").transform.position;
    }
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        meleeTimer = 0f;
        gameManager = GameManager.Instance;
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
            else{
                playerAnimator.ResetTrigger("Attack");
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

    void HandleMelee()
    {
        if ((SceneManager.GetActiveScene().name == "Casino") || (SceneManager.GetActiveScene().name == "TutorialLevel") || (SceneManager.GetActiveScene().name == "MainMenu")) {
            return;
        }
        if (Input.GetMouseButtonDown(0) && meleeTimer <= 0 && hasBone)
        {
            Vector3 attackDirection;
            Transform throwPoint = meleePointRight; // Default to right
            
            // Determine attack direction
            if (Input.GetKey(KeyCode.W))
            {
                attackDirection = Vector3.up;
                throwPoint = meleePointUp;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                attackDirection = Vector3.down;
                throwPoint = meleePointDown;
            }
            else
            {
                attackDirection = isFacingRight ? Vector3.right : Vector3.left;
                throwPoint = isFacingRight ? meleePointRight : meleePointLeft;
            }

            ThrowBone(throwPoint, attackDirection);
            meleeTimer = boneCooldown;
            playerAnimator.SetTrigger("Attack");
        }
    }

    void ThrowBone(Transform throwPoint, Vector3 direction)
    {
        // Disable held bone
        hasBone = false;
        heldBone.SetActive(false);

        // Instantiate and throw projectile
        GameObject bone = Instantiate(boneProjectilePrefab, throwPoint.position, Quaternion.identity);
        bone.transform.rotation = Quaternion.Euler(90, 0, 0);
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        
        // Set projectile direction
        if (rb != null)
        {
            rb.linearVelocity = direction * throwForce;
        }

        // Start cooldown coroutine
        StartCoroutine(ResetBone());
    }

    IEnumerator ResetBone()
    {
        yield return new WaitForSeconds(boneCooldown);
        hasBone = true;
        heldBone.SetActive(true);
    }

}
