using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Dialogue dialogueScript; // Reference to the Dialogue script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueScript.StartDialogue();
        }
    }

}
