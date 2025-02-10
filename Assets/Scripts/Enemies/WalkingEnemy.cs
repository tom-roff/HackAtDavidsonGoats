using UnityEngine;

public class WalkingEnemy : Enemy
{
    [Header("Platform Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    protected override void InitializeEnemy()
    {
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
        }
    }

    protected override void Move()
    {
        // Check for platform edges in 3D
        bool hasGround = Physics.Raycast(groundCheck.position, Vector3.down, checkDistance, groundLayer);
        bool hasForwardGround = Physics.Raycast(
            groundCheck.position, 
            movementDirection, 
            checkDistance, 
            groundLayer
        );

        if (!hasGround || hasForwardGround)
        {
            Flip();
        }

        transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + (movementDirection * checkDistance));
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);
        }
    }
}
