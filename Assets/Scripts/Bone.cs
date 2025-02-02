using UnityEngine;

public class Bone : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem impactParticles;

    void Start()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if (impactParticles)
            {
                ParticleSystem instance = Instantiate(impactParticles, transform.position, Quaternion.identity);
                instance.Play();
                Destroy(instance.gameObject, instance.main.duration);
            }

            Destroy(gameObject);
        }
    }
}
