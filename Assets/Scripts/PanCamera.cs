using UnityEngine;

public class PanCamera : MonoBehaviour
{
    [Header("Pan Settings")]
    [SerializeField] private float minAngle = 30f;
    [SerializeField] private float maxAngle = 35f;
    [SerializeField] private float cyclesPerSecond = 0.5f; // How many full cycles per second
    
    private float angleRange;
    private float midAngle;

    void Start()
    {
        angleRange = (maxAngle - minAngle) * 0.5f;
        midAngle = minAngle + angleRange;
    }

    void Update()
    {
        // Calculate angle using sine wave
        float angle = midAngle + Mathf.Sin(Time.time * cyclesPerSecond * 2f * Mathf.PI) * angleRange;
        
        // Apply rotation
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            angle,
            transform.rotation.eulerAngles.z
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 minRotation = Quaternion.Euler(0, minAngle, 0) * Vector3.forward;
        Vector3 maxRotation = Quaternion.Euler(0, maxAngle, 0) * Vector3.forward;
        
        Gizmos.DrawRay(transform.position, minRotation * 5f);
        Gizmos.DrawRay(transform.position, maxRotation * 5f);
    }
}
