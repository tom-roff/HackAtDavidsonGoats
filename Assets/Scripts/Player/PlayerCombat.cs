using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    // **** Melee Settings ****
    [Header("Melee Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform meleePointRight;
    [SerializeField] private Transform meleePointLeft;
    [SerializeField] private Transform meleePointUp;
    [SerializeField] private Transform meleePointDown;
    [SerializeField] private float meleeRange = 1.0f;
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private float meleeCooldown = 0.5f;

    // **** Ranged Settings ****
    [Header("Ranged Settings")]
    // This prefab can change based on the player's equipped ranged weapon.
    [SerializeField] private GameObject equippedProjectilePrefab;
    [SerializeField] private float rangedForce = 10f;
    [SerializeField] private float rangedCooldown = 1f;

    // **** Internal Variables ****
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;
    private PlayerManager playerManager;
    
    // Current mode: true for melee, false for ranged.
    private bool isMeleeMode = true;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    private void Update()
    {
        // Update cooldown timers.
        if (meleeTimer > 0)
            meleeTimer -= Time.deltaTime;
        if (rangedTimer > 0)
            rangedTimer -= Time.deltaTime;

        // Do not allow combat in non-combat scenes.
        if (IsInNonCombatScene())
            return;

        // Toggle between melee and ranged mode with Q.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isMeleeMode = !isMeleeMode;
            Debug.Log("Switched to " + (isMeleeMode ? "Melee Mode" : "Ranged Mode"));
        }

        // Use left mouse button to attack.
        if (Input.GetMouseButtonDown(0))
        {
            if (isMeleeMode)
            {
                if (meleeTimer <= 0)
                    HandleMelee();
            }
            else
            {
                if (rangedTimer <= 0)
                    HandleRanged();
            }
        }
    }

    private bool IsInNonCombatScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "Casino" || sceneName == "TutorialLevel" || sceneName == "MainMenu";
    }

    // **** Melee Attack ****
    private void HandleMelee()
    {
        // Determine the direction and corresponding spawn point.
        Vector3 attackDirection;
        Transform attackPoint = DetermineAttackPoint(out attackDirection);

        // Cast a ray from the spawn point in the attack direction.
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackDirection, out hit, meleeRange, enemyLayer))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }

        meleeTimer = meleeCooldown;
        // playerManager.PlayerAnimator.SetTrigger("MeleeAttack");
    }

    // **** Ranged Attack ****
    private void HandleRanged()
    {
        // Determine the direction and corresponding spawn point.
        Vector3 attackDirection;
        Transform attackPoint = DetermineAttackPoint(out attackDirection);

        // Instantiate the projectile at the same attack point.
        GameObject projectile = Instantiate(equippedProjectilePrefab, attackPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = attackDirection * rangedForce;
        }

        rangedTimer = rangedCooldown;
        // playerManager.PlayerAnimator.SetTrigger("RangedAttack");
    }

    // Returns the proper spawn transform based on input or player's facing.
    // If W or S is pressed, it uses the upward or downward transform; otherwise left/right.
    private Transform DetermineAttackPoint(out Vector3 attackDirection)
    {
        if (Input.GetKey(KeyCode.W))
        {
            attackDirection = Vector3.up;
            return meleePointUp;
        }
        if (Input.GetKey(KeyCode.S))
        {
            attackDirection = Vector3.down;
            return meleePointDown;
        }

        attackDirection = playerManager.Movement.IsFacingRight ? Vector3.right : Vector3.left;
        return playerManager.Movement.IsFacingRight ? meleePointRight : meleePointLeft;
    }

    // Draws rays from the melee points for debugging purposes.
    private void OnDrawGizmosSelected()
    {
        if (meleePointRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(meleePointRight.position, Vector3.right * meleeRange);
        }
        if (meleePointLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(meleePointLeft.position, Vector3.left * meleeRange);
        }
        if (meleePointUp != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(meleePointUp.position, Vector3.up * meleeRange);
        }
        if (meleePointDown != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(meleePointDown.position, Vector3.down * meleeRange);
        }
    }
}
