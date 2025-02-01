using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected float moveSpeed = 2f;
    
    [SerializeField] protected int currentHealth;
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
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0)
        {
            if (gameManager != null)
            {
                gameManager.DamagePlayer(contactDamage);
                damageTimer = damageCooldown;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0)
        {
            if (gameManager != null)
            {
                gameManager.DamagePlayer(contactDamage);
                damageTimer = damageCooldown;
            }
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        movementDirection *= -1;
        
        // Flip visual model without affecting collider
        Transform model = transform.Find("Model");
        if (model != null)
        {
            Vector3 newScale = model.localScale;
            newScale.x *= -1;
            model.localScale = newScale;
        }
    }
}
