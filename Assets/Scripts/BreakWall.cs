using UnityEngine;

public class BreakWall : MonoBehaviour
{
    [SerializeField] private GameObject breakableWall;
    [SerializeField] private GameObject particles;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(particles, breakableWall.transform.position, Quaternion.identity);
            Destroy(breakableWall);
        }
    }
}
