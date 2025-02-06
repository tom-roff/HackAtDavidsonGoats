using UnityEngine;

public class BoneClatter : MonoBehaviour
{
    private void OnParticleCollision(GameObject other) {
        SoundManager.Instance.PlaySound("BoneCrack", other.transform.position);
    }
}
