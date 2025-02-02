using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;

    public Animator walkingEnemyAnimator;
    
    [Header("Health Settings")]
    [SerializeField] protected int healthGiven = 1;
    protected int currentHealth;


    
    protected Transform player;
    protected bool facingRight = true;
    protected Vector3 movementDirection = Vector3.right;
    protected GameManager gameManager;
    
    private float damageTimer = 0f;
    [SerializeField] private float damageCooldown = 0.5f;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = GameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        
        InitializeEnemy();
    }

    protected virtual void InitializeEnemy() { }

    void Update()
    {
        Move();
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }

    walkingEnemyAnimator.SetFloat("Speed", .5f);

    }

    protected abstract void Move();

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (gameManager != null)
        {
            gameManager.HealPlayer(healthGiven);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandlePlayerCollision(other);
    }

    void OnTriggerStay(Collider other)
    {
        HandlePlayerCollision(other);
    }


    private void HandlePlayerCollision(Collider other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && !playerController.isDashing)
            {
                if (gameManager != null)
                {
                    // Apply damage
                    gameManager.DamagePlayer(contactDamage);
                    
                    // Calculate knockback direction
                    Vector3 knockbackDirection = CalculateKnockbackDirection(other.transform.position);
                    
                    // Apply knockback
                    playerController.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
                    
                    damageTimer = damageCooldown;
                }
            }
        }
    }

    private Vector3 CalculateKnockbackDirection(Vector3 playerPosition)
    {
        // Get horizontal direction based on relative position
        float horizontalDirection = Mathf.Sign(playerPosition.x - transform.position.x);
        
        // Always include upward component and prevent pure vertical knockback
        return new Vector3(horizontalDirection, 0.4f, 0).normalized;
    }


    protected void Flip()
    {
        facingRight = !facingRight;
        movementDirection *= -1;

        
        Vector3 rotationToRotate = new Vector3(0f,180f,0f);

        this.transform.Rotate(rotationToRotate);
        
        Transform model = transform.Find("Model");
        if (model != null)
        {
            Vector3 newScale = model.localScale;
            newScale.x *= -1;
            model.localScale = newScale;
        }
    }
}
