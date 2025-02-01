using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private AudioSource breakSound; // Optional

    // Version 1: Using Collision (More physical feeling)
    // private void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log("break begin");
    //     Debug.Log(other);
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("break");
    //         Break();
    //     }
    // }

    // Version 2: Using Trigger (More immediate/arcade feeling)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Break();
        }
    }

    private void Break()
    {
        // Spawn particle effect
        if (breakEffect != null)
        {
            // Detach the particle system so it continues playing after platform is destroyed
            breakEffect.transform.parent = null;
            breakEffect.Play();
            Destroy(breakEffect.gameObject, breakEffect.main.duration);
        }

        // Play sound if assigned
        if (breakSound != null)
        {
            breakSound.transform.parent = null;
            breakSound.Play();
            Destroy(breakSound.gameObject, breakSound.clip.length);
        }

        // Destroy the platform immediately
        Destroy(gameObject);
    }
}
