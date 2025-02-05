using UnityEngine;

public class BoneClatter : MonoBehaviour
{
    private void OnParticleCollision(GameObject other) {
        Debug.Log("clatter");
        SoundManager.Instance.PlaySound("BoneCrack", other.transform.position);
    }
}
