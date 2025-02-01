using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
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

    void OnTriggerEnter(Collider collision)
    {
        HandlePlayerCollision(collision.GetComponent<Collider>());
    }

    void OnTriggerStay(Collider collision)
    {
        HandlePlayerCollision(collision.GetComponent<Collider>());
    }

    private void HandlePlayerCollision(Collider other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0)
        {
            if (gameManager != null)
            {
                // Apply damage
                gameManager.DamagePlayer(contactDamage);
                
                // Apply knockback
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                other.GetComponent<PlayerController>()?.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
                
                damageTimer = damageCooldown;
            }
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        movementDirection *= -1;
        
        Transform model = transform.Find("Model");
        if (model != null)
        {
            Vector3 newScale = model.localScale;
            newScale.x *= -1;
            model.localScale = newScale;
        }
    }
}
