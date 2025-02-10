using UnityEngine;

public class HourglassMove : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 0.5f;    // How far it moves up/down
    [SerializeField] private float frequency = 1f;      // How many cycles per second
    [SerializeField] private float offset = 0f;         // Starting position in the cycle

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        offset = Random.Range(0f, 2f * Mathf.PI); // Random start position in cycle
    }

    void Update()
    {
        // Calculate new Y position using sine wave
        float newY = startPosition.y + amplitude * Mathf.Sin((Time.time + offset) * frequency * 2f * Mathf.PI);
        
        // Apply new position
        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}
