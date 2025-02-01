using UnityEngine;

public class SwingingObject : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private float swingAngle = 45f;
    [SerializeField] private float swingSpeed = 2f;

    [Header("Death Settings")]
    [SerializeField] private Vector3 swingAxis = Vector3.forward; // Changed to forward for proper pendulum motion

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float timeOffset;

    void Start()
    {
        // Store initial positions relative to pivot
        initialPosition = transform.position - pivotPoint.position;
        initialRotation = transform.rotation;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Calculate rotation angle using sine wave
        float angle = swingAngle * Mathf.Sin((Time.time + timeOffset) * swingSpeed);
        
        // Rotate around pivot point
        transform.position = pivotPoint.position + Quaternion.Euler(swingAxis * angle) * initialPosition;
        transform.rotation = Quaternion.Euler(swingAxis * angle) * initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the GameManager instance properly
            GameManager gameManager = GameManager.Instance;
            
            if (gameManager != null)
            {
                // Handle CharacterController conflict
                CharacterController controller = other.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    other.transform.position = gameManager.GetCurrentCheckpoint();
                    controller.enabled = true;
                }
                else
                {
                    gameManager.KillPlayer();
                }
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        if (pivotPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pivotPoint.position, 0.1f);
            Gizmos.DrawLine(pivotPoint.position, transform.position);
            
            // Draw swing arc
            Vector3 startDir = Quaternion.Euler(swingAxis * -swingAngle) * (transform.position - pivotPoint.position);
            Vector3 endDir = Quaternion.Euler(swingAxis * swingAngle) * (transform.position - pivotPoint.position);
            
            Gizmos.DrawLine(pivotPoint.position, pivotPoint.position + startDir);
            Gizmos.DrawLine(pivotPoint.position, pivotPoint.position + endDir);
        }
    }
}
