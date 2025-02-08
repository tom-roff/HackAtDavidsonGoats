using UnityEngine;
using System.Collections;

public class BoneClatter : MonoBehaviour
{
    public float delayBetweenSounds;

    private bool canPlaySound = true;

    private void OnParticleCollision(GameObject other)
    {
        if (canPlaySound)
        {
            StartCoroutine(PlaySoundWithDelay(other));
        }
    }

    private IEnumerator PlaySoundWithDelay(GameObject other)
    {
        delayBetweenSounds = Random.Range(2, 3) / 10f;
        
        canPlaySound = false;

        int randSegment = Random.Range(0, 10);
        float startNormalized = randSegment / 10f;
        float endNormalized = startNormalized + 0.2f;

        SoundManager.Instance.PlaySound("Bone", other.transform.position, startNormalized, endNormalized);

        yield return new WaitForSeconds(delayBetweenSounds);

        canPlaySound = true;
    }
}
