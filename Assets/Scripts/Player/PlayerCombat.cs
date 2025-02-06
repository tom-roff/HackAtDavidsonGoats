using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform meleePointRight;
    [SerializeField] private Transform meleePointLeft;
    [SerializeField] private Transform meleePointUp;
    [SerializeField] private Transform meleePointDown;

    [Header("Bone Settings")]
    [SerializeField] private GameObject boneProjectilePrefab;
    [SerializeField] private GameObject heldBone;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float boneCooldown = 1f;

    private bool hasBone = true;
    private float meleeTimer;
    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        meleeTimer = 0f;
    }

    private void Update()
    {
        if (meleeTimer > 0)
        {
            meleeTimer -= Time.deltaTime;
        }
        else
        {
            playerManager.PlayerAnimator.ResetTrigger("Attack");
        }

        HandleMelee();
    }

    private void HandleMelee()
    {
        if (IsInNonCombatScene()) return;

        if (Input.GetMouseButtonDown(0) && meleeTimer <= 0 && hasBone)
        {
            Vector3 attackDirection;
            Transform throwPoint = DetermineThrowPoint(out attackDirection);

            ThrowBone(throwPoint, attackDirection);
            meleeTimer = boneCooldown;
            playerManager.PlayerAnimator.SetTrigger("Attack");
        }
    }

    private bool IsInNonCombatScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "Casino" || sceneName == "TutorialLevel" || sceneName == "MainMenu";
    }

    private Transform DetermineThrowPoint(out Vector3 attackDirection)
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

    private void ThrowBone(Transform throwPoint, Vector3 direction)
    {
        hasBone = false;
        heldBone.SetActive(false);

        GameObject bone = Instantiate(boneProjectilePrefab, throwPoint.position, Quaternion.Euler(90, 0, 0));
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * throwForce;
        }

        StartCoroutine(ResetBone());
    }

    private IEnumerator ResetBone()
    {
        yield return new WaitForSeconds(boneCooldown);
        hasBone = true;
        heldBone.SetActive(true);
    }
}
